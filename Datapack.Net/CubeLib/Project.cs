using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Pack;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Datapack.Net.Function.Commands.Execute.Subcommand;

namespace Datapack.Net.CubeLib
{
    public abstract class Project
    {
        public static Project ActiveProject { get; private set; }
        public static readonly List<Project> Projects = [];
        public static readonly Storage GlobalStorage = new(new("cubelib", "global"));
        public static readonly NamedTarget GlobalScoreEntity = new("#_cubelib_score");

        public readonly DP Datapack;
        public abstract string Namespace { get; }

        private readonly Dictionary<Action, MCFunction> MCFunctions = [];
        private readonly List<MCFunction> MiscMCFunctions = [];
        private readonly Queue<KeyValuePair<Action, MCFunction>> FunctionsToProcess = [];

        private readonly HashSet<Score> Scores = [];
        private readonly HashSet<Score> Registers = [];
        private readonly HashSet<Score> Globals = [];
        private readonly List<IStaticType> StaticTypes = [];
        private readonly List<Command> MiscInitCmds = [];

        private MCFunction? currentTarget;
        public MCFunction CurrentTarget { get => currentTarget ?? throw new InvalidOperationException("Project not building yet"); private set => currentTarget = value; }
        public MCFunction CurrentTargetCleanup { get; private set; }
        public DeclareMCAttribute CurrentFunctionAttrs { get; private set; }

        private List<ScoreRef> RegistersInUse = [];

        public readonly NamedTarget ScoreEntity;
        public readonly Storage InternalStorage;

        public MCStack RegisterStack;
        public MCHeap Heap;

        private int AnonymousFuncCounter = 0;

        public readonly List<Project> Dependencies = [];

        private bool Built = false;

        public CubeLibStd Std;

        protected Project(DP pack)
        {
            Datapack = pack;
            ScoreEntity = new($"#_{Namespace}_cubelib_score");
            InternalStorage = new(new(Namespace, "_internal"));
        }

        public static T Create<T>(DP pack) where T : Project
        {
            T project = (T?)Activator.CreateInstance(typeof(T), [pack]) ?? throw new ArgumentException("Invalid project constructor");
            var index = Projects.FindIndex((i) => i.GetType() == project.GetType());
            if (index == -1)
            {
                Projects.Add(project);
                return project;
            }
            return (T)Projects[index];
        }

        public T AddDependency<T>() where T : Project
        {
            var project = Create<T>(Datapack);
            project.Build();
            ActiveProject = this;
            Dependencies.Add(project);
            return project;
        }

        public void Build()
        {
            if (Built) return;
            Built = true;
            ActiveProject = this;

            Std = AddDependency<CubeLibStd>();
            Init();

            AddFunction(Main);
            AddFunction(Tick);

            foreach (var i in GetType().GetMethods())
            {
                if (i.GetCustomAttribute<DeclareMCAttribute>() is not null) AddFunction((Action)Delegate.CreateDelegate(typeof(Action), this, i));
            }

            var tags = Datapack.GetResource<Tags>();
            var mainTag = tags.GetTag(new("minecraft", "load"), "functions");
            var tickTag = tags.GetTag(new("minecraft", "tick"), "functions");

            mainTag.Values.Add(new(Namespace, "_main"));
            tickTag.Values.Add(new(Namespace, "_tick"));

            RegisterStack = new(GlobalStorage, "register_stack");
            AddStaticObject(RegisterStack);
            Heap = new(InternalStorage, "heap");
            AddStaticObject(Heap);

            while (FunctionsToProcess.TryDequeue(out var i))
            {
                RegistersInUse.Clear();
                CurrentTarget = i.Value;
                CurrentTargetCleanup = new MCFunction(new(i.Value.ID.Namespace, $"zz_cleanup/{i.Value.ID.Path}"), true);
                CurrentFunctionAttrs = DeclareMCAttribute.Get(i.Key);
                MiscMCFunctions.Add(CurrentTargetCleanup);
                i.Key();

                RegistersInUse.Reverse();

                WithCleanup(() =>
                {
                    foreach (var r in RegistersInUse)
                    {
                        RegisterStack.Dequeue(r);
                    }
                });

                Call(CurrentTargetCleanup);
            }

            // Prepend Main
            foreach (var i in StaticTypes)
            {
                i.Init();
            }

            foreach (var i in MiscInitCmds)
            {
                PrependMain(i);
            }

            foreach (var i in Registers)
            {
                PrependMain(new Scoreboard.Players.Set(ScoreEntity, i, 0));
            }

            foreach (var i in Scores.Reverse())
            {
                PrependMain(new Scoreboard.Objectives.Add(i));
            }

            foreach (var i in MCFunctions.Values)
            {
                Datapack.GetResource<Functions>().Add(i);
            }

            foreach (var i in MiscMCFunctions)
            {
                Datapack.GetResource<Functions>().Add(i);
            }
        }

        public MCFunction GuaranteeFunc(Action func, bool macro)
        {
            var attr = DeclareMCAttribute.Get(func);
            MCFunction retFunc;
            if (func.Target != this && func.Target is Project lib)
            {
                if (lib.Built) return lib.MCFunctions[func];
                retFunc = new(new(lib.Namespace, attr.Path), true);
            }
            else if (FindFunction(func) is MCFunction mcfunc)
            {
                retFunc = mcfunc;
            }
            else retFunc = AddFunction(func);

            if (macro && attr.Macros.Length == 0) throw new ArgumentException("Attempted to call a non macro function with arguments");
            if (!macro && attr.Macros.Length != 0) throw new ArgumentException("Attempted to call a macro function without arguments");

            return retFunc;
        }

        public void Call(Action func)
        {
            Call(GuaranteeFunc(func, false));
        }

        public void Call(MCFunction func) => AddCommand(new FunctionCommand(func));

        public void Call(Action func, Storage storage, string path = "", bool macro = false) => Call(GuaranteeFunc(func, true), storage, path, macro);
        public void Call(MCFunction func, Storage storage, string path = "", bool macro = false) => AddCommand(new FunctionCommand(func, storage, path, macro));

        public void Call(Action func, KeyValuePair<string, object>[] args, bool macro = false) => Call(GuaranteeFunc(func, true), args, macro);
        public void Call(MCFunction func, KeyValuePair<string, object>[] args, bool macro = false)
        {
            AddCommand(BaseCall(func, args, macro));
        }

        public ScoreRef CallRet(Action func)
        {
            var ret = Local();
            CallRet(func, ret);
            return ret;
        }

        public void CallRet(Action func, ScoreRef ret)
        {
            if (!DeclareMCAttribute.Get(func).Returns) throw new InvalidOperationException("Function does not return a value");

            CallRet(GuaranteeFunc(func, false), ret);
        }

        public void CallRet(MCFunction func, ScoreRef ret, bool macro = false) => AddCommand(new Execute(macro).Store(ret).Run(new FunctionCommand(func)));

        public void CallRet(Action func, ScoreRef ret, Storage storage, string path = "", bool macro = false) => CallRet(GuaranteeFunc(func, true), ret, storage, path, macro);
        public void CallRet(MCFunction func, ScoreRef ret, Storage storage, string path = "", bool macro = false) => AddCommand(new Execute(macro).Store(ret).Run(new FunctionCommand(func, storage, path)));

        public void CallRet(Action func, ScoreRef ret, KeyValuePair<string, object>[] args, bool macro = false, int tmp = 0) => CallRet(GuaranteeFunc(func, true), ret, args, macro, tmp);

        public ScoreRef CallRet(Action func, KeyValuePair<string, object>[] args, bool macro = false, int tmp = 0)
        {
            var ret = Local();
            CallRet(func, ret, args, macro, tmp);
            return ret;
        }

        public void CallRet(MCFunction func, ScoreRef ret, KeyValuePair<string, object>[] args, bool macro = false, int tmp = 0)
        {
            AddCommand(new Execute(macro).Store(ret).Run(BaseCall(func, args, macro, tmp)));
        }

        public FunctionCommand BaseCall(MCFunction func, KeyValuePair<string, object>[] args, bool macro = false, int tmp = 0)
        {
            var parameters = new NBTCompound();
            var runtimeScores = new Dictionary<string, ScoreRef>();

            foreach (var i in args)
            {
                if (i.Value is string str) parameters[i.Key] = str;
                else if (i.Value is int val) parameters[i.Key] = val;
                else if (i.Value is ScoreRef score) runtimeScores[i.Key] = score;
                else if (i.Value is NamespacedID id) parameters[i.Key] = id.ToString();
                else if (i.Value is Storage storage) parameters[i.Key] = storage.ToString();
                else throw new ArgumentException($"Type {i.Value.GetType().Name} is not supported yet");
            }

            if (runtimeScores.Count == 0)
            {
                return new FunctionCommand(func, parameters, macro);
            }

            AddCommand(new DataCommand.Modify(InternalStorage, $"tmp{tmp}", macro).Set().Value(parameters.ToString()));
            foreach (var i in runtimeScores)
            {
                AddCommand(new Execute(macro).Store(InternalStorage, $"tmp{tmp}.{i.Key}", NBTNumberType.Int, 1).Run(i.Value.Get()));
            }

            return new FunctionCommand(func, InternalStorage, $"tmp{tmp}");
        }

        public void Print(HeapPointer ptr)
        {
            Call(Std.PointerPrint, ptr.StandardMacros());
        }

        public void Print(params object[] args)
        {
            var text = new FormattedText();

            foreach (var i in args)
            {
                if (i is string str) text.Text(str);
                else if (i is ScoreRef score) text.Score(score);
                else throw new ArgumentException($"Invalid print object {i}");

                text.Text(" ");
            }

            text.RemoveLast();

            AddCommand(new TellrawCommand(new TargetSelector(TargetType.a), text));
        }

        public void Return(int val) => AddCommand(new ReturnCommand(val));
        public void Return() => Return(1);

        /// <summary>
        /// Returns from the function with the result of the command. Local variables are invalidated. <br/>
        /// Use <see cref="Return(ScoreRef)"/> to return a local variable.
        /// </summary>
        /// <param name="cmd"></param>
        public void Return(Command cmd) => AddCommand(new ReturnCommand(cmd));
        public void Return(ScoreRef score)
        {
            var temp = Temp(0, "ret");
            temp.Set(score);
            Return(temp.Get());
        }

        public void ReturnFail() => AddCommand(new ReturnCommand());

        public void Break() => Return();

        public MCFunction? FindFunction(Action func)
        {
            var actions = MCFunctions.ToList();
            var index = actions.FindIndex((i) => i.Key.Method.MethodHandle.Equals(func.Method.MethodHandle));
            if (index != -1) return actions[index].Value;
            return null;
        }

        public MCFunction AddFunction(Action func)
        {
            return AddFunction(func, new(Namespace, DeclareMCAttribute.Get(func).Path));
        }

        public MCFunction AddFunction(Action func, NamespacedID id, bool scoped = false)
        {
            var mcfunc = new MCFunction(id, true);
            MCFunctions[func] = mcfunc;

            if (scoped)
            {
                var cleanup = new MCFunction(new(id.Namespace, $"zz_cleanup/{id.Path}"), true);
                MiscMCFunctions.Add(cleanup);

                List<ScoreRef> scope = [.. RegistersInUse];
                var oldFunc = CurrentTarget;
                var oldCleanup = CurrentTargetCleanup;
                CurrentTarget = mcfunc;
                CurrentTargetCleanup = cleanup;

                func();

                WithCleanup(() =>
                {
                    for (int i = RegistersInUse.Count - 1; i >= scope.Count; i--)
                    {
                        RegisterStack.Dequeue(RegistersInUse[i]);
                    }
                });

                Call(CurrentTargetCleanup);

                RegistersInUse = scope;
                CurrentTarget = oldFunc;
                CurrentTargetCleanup = oldCleanup;
            }
            else
            {
                FunctionsToProcess.Enqueue(new(func, mcfunc));
            }
            return mcfunc;
        }

        public void AddCommand(Command cmd)
        {
            if (cmd is ReturnCommand) Call(CurrentTargetCleanup);
            else if (cmd is Execute ex && ex.Get<Run>().Command is ReturnCommand)
            {
                var other = ex.Copy();
                other.RemoveAll<Run>();
                other.Run(new FunctionCommand(CurrentTargetCleanup));
                AddCommand(other);
            }

            CurrentTarget.Add(cmd);
        }

        public void WithCleanup(Action func)
        {
            var oldTarget = CurrentTarget;
            CurrentTarget = CurrentTargetCleanup;

            func();

            CurrentTarget = oldTarget;
        }

        public void AddStaticObject(IStaticType type) => StaticTypes.Add(type);

        public void PrependMain(Command cmd)
        {
            (FindFunction(Main) ?? throw new Exception("Main doesn't exist")).Prepend(cmd);
        }

        public void AddScore(Score score) => Scores.Add(score);

        public ScoreRef Temp(int num, string type = "def")
        {
            var score = new Score($"_cl_tmp_{type}_{num}", "dummy");
            if (!Scores.Contains(score)) AddScore(score);
            return new(score, ScoreEntity);
        }

        public ScoreRef Temp(int num, int val, string type = "def")
        {
            var tmp = Temp(num, type);
            tmp.Set(val);
            return tmp;
        }

        public ScoreRef Local()
        {
            var score = new Score($"_cl_reg_{RegistersInUse.Count}", "dummy");
            Registers.Add(score);
            Scores.Add(score);

            var register = new ScoreRef(score, GlobalScoreEntity);
            RegistersInUse.Add(register);

            RegisterStack.Enqueue(register);

            return register;
        }

        public ScoreRef Local(int val)
        {
            var register = Local();
            register.Set(val);
            return register;
        }

        public ScoreRef Local(ScoreRef val)
        {
            var register = Local();
            register.Set(val);
            return register;
        }

        public ScoreRef Global()
        {
            var score = new Score($"_cl_{Namespace}_var_{Globals.Count}", "dummy");
            Globals.Add(score);
            Scores.Add(score);

            return new ScoreRef(score, GlobalScoreEntity);
        }

        public ScoreRef Global(int val)
        {
            var global = Global();
            MiscInitCmds.Add(new Scoreboard.Players.Set(global.Target, global.Score, val));
            return global;
        }

        public FunctionCommand Lambda(Action func)
        {
            var mcfunc = AddFunction(func, new(Namespace, $"zz_anon/{AnonymousFuncCounter++}"), true);

            if (CurrentFunctionAttrs.Macros.Length == 0) return new(mcfunc);

            var nbt = new NBTCompound();

            foreach (var i in CurrentFunctionAttrs.Macros)
            {
                nbt[i] = $"$({i})";
            }

            return new(mcfunc, nbt, true);
        }

        public HeapPointer Alloc(ScoreRef loc) => Alloc(loc, 0);

        public HeapPointer Alloc(ScoreRef loc, int val)
        {
            var pointer = Heap.Alloc(loc);
            pointer.Set(val);
            return pointer;
        }

        public HeapPointer Attach(ScoreRef loc) => new(Heap, loc);

        public HeapPointer AllocIfNull(ScoreRef loc, int val = 0)
        {
            var ptr = Attach(loc);
            If(!ptr.Exists(), () => Alloc(ptr, val));
            return ptr;
        }

        public IfHandler If(Conditional comp, Action res) => new(this, comp, res);
        public void If(Conditional comp, Command res) => AddCommand(comp.Process(new Execute()).Run(res));

        public void While(Conditional comp, Action res)
        {
            WhileTrue(() =>
            {
                If(!comp, new ReturnCommand(0));
                res();
            });
        }

        public void WhileTrue(Action res)
        {
            var func = Lambda(() =>
            {
                res();
                Call(CurrentTarget);
            });
            AddCommand(func);
        }

        public void For(int start, ScoreRef end, Action<ScoreRef> res)
        {
            var i = Local(start);
            While(i < end, () =>
            {
                res(i);
                i.Add(1);
            });
        }

        public void Random(MCRange<int> range, ScoreRef score) => AddCommand(new Execute().Store(score).Run(new RandomCommand(range)));

        public ScoreRef Random(MCRange<int> range)
        {
            var score = Local();
            Random(range, score);
            return score;
        }

        protected virtual void Init() { }

        [DeclareMC("_main")]
        protected virtual void Main() { }

        [DeclareMC("_tick")]
        protected virtual void Tick() { }
    }
}
