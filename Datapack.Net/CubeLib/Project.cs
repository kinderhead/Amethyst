using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.CubeLib.Builtins.Static;
using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Pack;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Datapack.Net.Data._1_20_4.Blocks;
using static Datapack.Net.Function.Commands.Execute.Subcommand;

namespace Datapack.Net.CubeLib
{
    public abstract partial class Project
    {
        public static Project ActiveProject { get; private set; }
        public static readonly List<Project> Projects = [];
        public static readonly Storage GlobalStorage = new(new("cubelib", "global"));
        public static readonly NamedTarget GlobalScoreEntity = new("#_cubelib_score");
        public static readonly Score EntityIDScore = new("_cl_id", "dummy");
        public static Settings Settings;

        internal static Dictionary<Delegate, MCFunction> RuntimeMethods = [];

        public readonly DP Datapack;
        public abstract string Namespace { get; }

        private readonly Dictionary<Delegate, MCFunction> MCFunctions = [];
        private static readonly Dictionary<NamespacedID, MCFunction> MacroCheckFunctions = [];
        private readonly List<MCFunction> MiscMCFunctions = [];
        private readonly Queue<KeyValuePair<Delegate, MCFunction>> FunctionsToProcess = [];

        private readonly HashSet<Score> Scores = [];
        private readonly HashSet<Score> Registers = [];
        private readonly HashSet<Score> Globals = [];
        private readonly List<IStaticType> StaticTypes = [];
        private readonly List<Command> MiscInitCmds = [];
        private readonly Dictionary<int, ScoreRef> Constants = [];

        private MCFunction? currentTarget;
        public MCFunction CurrentTarget { get => currentTarget ?? throw new InvalidOperationException("Project not building yet"); private set => currentTarget = value; }
        public MCFunction CurrentTargetCleanup { get; private set; }
        public DeclareMCAttribute CurrentFunctionAttrs { get; private set; }

        private List<ScoreRef> RegistersInUse = [];

        public readonly NamedTarget ScoreEntity;
        public readonly Storage InternalStorage;

        public MCStaticStack RegisterStack;
        public MCStaticStack ArgumentStack;
        public MCStaticStack StorageArgumentStack;
        public MCStaticHeap Heap;

        private int AnonymousFuncCounter = 0;

        public readonly List<Project> Dependencies = [];

        private bool BuiltOrBuilding = false;

        public CubeLibStd Std;

        public ScoreRef ErrorScore;

        public readonly List<MCFunction> DynamicFunctions = [];

        private bool SendToMisc = false;

        private bool DuringInit = false;

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
            if (BuiltOrBuilding) return;
            BuiltOrBuilding = true;
            ActiveProject = this;

            Std = AddDependency<CubeLibStd>();

            foreach (var i in GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if (i.GetCustomAttribute<DeclareMCAttribute>() is not null) AddFunction(DelegateUtils.Create(i, this));
            }

            var tags = Datapack.GetResource<Tags>();
            var mainTag = tags.GetTag(new("minecraft", "load"), "functions");
            var tickTag = tags.GetTag(new("minecraft", "tick"), "functions");

            mainTag.Values.Add(new(Namespace, "_main"));
            tickTag.Values.Add(new(Namespace, "_tick"));

            RegisterStack = new(GlobalStorage, "register_stack");
            AddStaticObject(RegisterStack);
            ArgumentStack = new(GlobalStorage, "argument_stack");
            AddStaticObject(ArgumentStack);
            StorageArgumentStack = new(GlobalStorage, "storage_stack");
            AddStaticObject(StorageArgumentStack);
            Heap = new(GlobalStorage, "heap");
            AddStaticObject(Heap);

            Scores.Add(EntityIDScore);

            if (Settings.ErrorChecking)
            {
                var score = new Score("_cl_err", "dummy");
                Scores.Add(score);
                ErrorScore = new(score, GlobalScoreEntity);
            }

            DuringInit = true;
            Init();
            DuringInit = false;

            while (FunctionsToProcess.TryDequeue(out var i))
            {
                RegistersInUse.Clear();
                CurrentTarget = i.Value;
                CurrentTargetCleanup = new MCFunction(new(i.Value.ID.Namespace, $"zz_cleanup/{i.Value.ID.Path}"), true);
                CurrentFunctionAttrs = DeclareMCAttribute.Get(i.Key);
                MiscMCFunctions.Add(CurrentTargetCleanup);

                List<IPointer> pointers = [];

                if (Settings.VerifyMacros && CurrentFunctionAttrs.Macros.Length > 0)
                {
                    var macroCheck = MacroCheckFunctions[CurrentTarget.ID];
                    MiscMCFunctions.Add(macroCheck);
                    foreach (var macro in CurrentFunctionAttrs.Macros)
                    {
                        macroCheck.Add(new Execute(true).Unless.Data(new StorageMacro("$(storage)"), $"$(path).{macro}").Run(new TellrawCommand(new TargetSelector(TargetType.a), new FormattedText().Text($"Macro argument {macro} was not provided to function {CurrentTarget.ID}"))));
                    }
                    MacroCheckFunctions[CurrentTarget.ID] = macroCheck;
                }

                var args = DeclareMCAttribute.Args(i.Key);
                if (args.Length == 0) i.Key.DynamicInvoke();
                else
                {
                    List<IRuntimeArgument> funcArgs = [];

                    foreach (var e in args)
                    {
                        if (e.IsAssignableTo(typeof(IBaseRuntimeObject)))
                        {
                            StorageArgumentStack.DequeueToStorage();
                            var ptr = Alloc<NBTCompound>();
                            Std.PointerSetFrom([.. ptr.StandardMacros(), new("src_storage", InternalStorage), new("src_path", "tmpstack_st")]);
                            funcArgs.Add(IBaseRuntimeObject.CreateWithRTP(ptr.Pointer, e));
                            pointers.Add(ptr);
                        }
                        else funcArgs.Add((IRuntimeArgument?)e.GetMethod("Create")?.Invoke(null, [ArgumentStack.Dequeue()]) ?? throw new ArgumentException($"Invalid arguments for function {i.Key.Method.Name}"));
                    }

                    i.Key.DynamicInvoke([.. funcArgs]);
                }

                RegistersInUse.Reverse();

                WithCleanup(() =>
                {
                    foreach (var p in pointers)
                    {
                        p.Free();
                    }

                    foreach (var r in RegistersInUse)
                    {
                        RegisterStack.Dequeue(r);
                    }
                });

                Call(CurrentTargetCleanup);
            }

            // Prepend Main
            foreach (var i in MiscInitCmds.Reverse<Command>())
            {
                PrependMain(i);
            }

            foreach (var i in StaticTypes)
            {
                i.Init();
            }

            foreach (var i in Constants)
            {
                PrependMain(new Scoreboard.Players.Set(i.Value.Target, i.Value.Score, i.Key));
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

            foreach (var i in DynamicFunctions)
            {
                Datapack.GetResource<Functions>().Add(i);
            }

            foreach (var i in MiscMCFunctions)
            {
                Datapack.GetResource<Functions>().Add(i);
            }
        }

        public MCFunction GuaranteeFunc(Delegate func, KeyValuePair<string, object>[] macros)
        {
            var attr = DeclareMCAttribute.Get(func);
            MCFunction retFunc;
            if (func.Target != this && func.Target is Project lib)
            {
                if (lib.BuiltOrBuilding) return lib.GuaranteeFunc(func, macros);
                retFunc = new(new(lib.Namespace, attr.Path), true);
            }
            else if (FindFunction(func) is MCFunction mcfunc) retFunc = mcfunc;
            else retFunc = AddFunction(func);

            if (macros.Length != 0 && attr.Macros.Length == 0) throw new ArgumentException("Attempted to call a non macro function with arguments");
            else if (macros.Length == 0 && attr.Macros.Length != 0) throw new ArgumentException("Attempted to call a macro function without arguments");
            else if (macros.Length != attr.Macros.Length) throw new ArgumentException("Mismatch in number of macro arguments passed to and accepted by function");
            else if (macros.Length != 0)
            {
                foreach (var i in macros)
                {
                    if (!attr.Macros.Contains(i.Key)) throw new ArgumentException($"Function does not accept macro argument \"{i.Key}\"");
                }
            }

            return retFunc;
        }

        public void Call(Action func, bool macro = false) => Call(GuaranteeFunc(func, []), macro);
        public void Call(MCFunction func, bool macro = false) => AddCommand(new FunctionCommand(func, macro));

        public void Call(Action func, KeyValuePair<string, object>[] args, bool macro = false, int tmp = 0) => Call(GuaranteeFunc(func, args), args, macro, tmp);
        public void Call(MCFunction func, KeyValuePair<string, object>[] args, bool macro = false, int tmp = 0) => AddCommand(BaseCall(func, args, macro, tmp));

        public void CallArg(Delegate func, IRuntimeArgument[] args, bool macro = false) => CallArg(GuaranteeFunc(func, []), args, macro);
        public void CallArg(Delegate func, IRuntimeArgument[] args, KeyValuePair<string, object>[] macros, bool macro = false, int tmp = 0) => CallArg(GuaranteeFunc(func, macros), args, macros, macro, tmp);
        public void CallArg(MCFunction func, IRuntimeArgument[] args, bool macro = false) => AddCommand(BaseCall(func, args, macro));
        public void CallArg(MCFunction func, IRuntimeArgument[] args, KeyValuePair<string, object>[] macros, bool macro = false, int tmp = 0) => AddCommand(BaseCall(func, args, macros, macro, tmp));

        public ScoreRef CallRet(Action func)
        {
            var ret = Local();
            CallRet(func, ret);
            return ret;
        }

        public void CallRet(Action func, ScoreRef ret, bool macro = false)
        {
            if (!DeclareMCAttribute.Get(func).Returns) throw new InvalidOperationException("Function does not return a value");

            CallRet(GuaranteeFunc(func, []), ret, macro);
        }

        public void CallRet(MCFunction func, ScoreRef ret, bool macro = false) => AddCommand(new Execute(macro).Store(ret).Run(new FunctionCommand(func)));

        public void CallRet(Action func, ScoreRef ret, KeyValuePair<string, object>[] args, bool macro = false, int tmp = 0) => CallRet(GuaranteeFunc(func, args), ret, args, macro, tmp);

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

        public void CallArgRet(Delegate func, ScoreRef ret, IRuntimeArgument[] args, bool macro = false) => CallArgRet(GuaranteeFunc(func, []), ret, args, macro);
        public void CallArgRet(Delegate func, ScoreRef ret, IRuntimeArgument[] args, KeyValuePair<string, object>[] macros, bool macro = false, int tmp = 0) => CallArgRet(GuaranteeFunc(func, macros), ret, args, macros, macro, tmp);
        public void CallArgRet(MCFunction func, ScoreRef ret, IRuntimeArgument[] args, bool macro = false) => AddCommand(new Execute(macro).Store(ret).Run(BaseCall(func, args, macro)));
        public void CallArgRet(MCFunction func, ScoreRef ret, IRuntimeArgument[] args, KeyValuePair<string, object>[] macros, bool macro = false, int tmp = 0) => AddCommand(new Execute().Store(ret).Run(BaseCall(func, args, macros, macro, tmp)));

        private FunctionCommand BaseCall(MCFunction func, KeyValuePair<string, object>[] args, bool macro = false, int tmp = 0)
        {
            var parameters = new NBTCompound();
            var runtimeScores = new Dictionary<string, ScoreRef>();
            var pointers = new Dictionary<string, IPointer>();

            foreach (var i in args)
            {
                if (NBTType.ToNBT(i.Value) != null) parameters[i.Key] = NBTType.ToNBT(i.Value) ?? throw new Exception("How did we get here?");
                else if (i.Value.GetType().IsAssignableTo(typeof(NBTType))) parameters[i.Key] = (NBTType)i.Value;
                else if (i.Value is ScoreRef score) runtimeScores[i.Key] = score;
                else if (i.Value is IToPointer ptr) pointers[i.Key] = ptr.ToPointer();
                else if (i.Value is NamespacedID id) parameters[i.Key] = id.ToString();
                else if (i.Value is Storage storage) parameters[i.Key] = storage.ToString();
                else throw new ArgumentException($"Type {i.Value.GetType().Name} is not supported yet");
            }

            if (runtimeScores.Count == 0 && pointers.Count == 0)
            {
                return new FunctionCommand(func, parameters, macro);
            }

            AddCommand(new DataCommand.Modify(InternalStorage, $"func_tmp{tmp}", macro).Set().Value(parameters.ToString()));

            foreach (var i in runtimeScores)
            {
                AddCommand(new Execute(macro).Store(InternalStorage, $"func_tmp{tmp}.{i.Key}", NBTNumberType.Int, 1).Run(i.Value.Get()));
            }

            foreach (var i in pointers)
            {
                Std.PointerDereference(i.Value.StandardMacros([new("dest_storage", InternalStorage), new("dest", $"func_tmp{tmp}.{i.Key}")]), macro, tmp + 1);
            }

            if (Settings.VerifyMacros && !func.ID.Path.StartsWith("zz_")) AddCommand(new FunctionCommand(MacroCheckFunctions[func.ID], new NBTCompound
            {
                ["storage"] = InternalStorage.ToString(),
                ["path"] = $"func_tmp{tmp}"
            }));

            return new FunctionCommand(func, InternalStorage, $"func_tmp{tmp}");
        }

        private FunctionCommand BaseCall(MCFunction func, IRuntimeArgument[] args, bool macro = false)
        {
            PushArgs(args);
            return new FunctionCommand(func, macro);
        }

        private FunctionCommand BaseCall(MCFunction func, IRuntimeArgument[] args, KeyValuePair<string, object>[] macros, bool macro = false, int tmp = 0)
        {
            PushArgs(args);
            return BaseCall(func, macros, macro, tmp);
        }

        public void PushArgs(IRuntimeArgument[] args)
        {
            foreach (var i in args.Reverse())
            {
                if (i is IBaseRuntimeObject obj)
                {
                    var rawPtr = obj.GetPointer();
                    RuntimePointer<NBTCompound> ptr;
                    bool needsFree = false;

                    if (rawPtr is BaseHeapPointer hp)
                    {
                        ptr = hp.ToRTP<NBTCompound>();
                        needsFree = true;
                    }
                    else ptr = (RuntimePointer<NBTCompound>)rawPtr.Cast<NBTCompound>();

                    StorageArgumentStack.Enqueue(ptr.SelfPointer);
                    if (needsFree) ptr.Free();
                }
                else ArgumentStack.Enqueue(i.GetAsArg());
            }
        }

        public void Print<T>(IPointer<T> ptr) => Std.PointerPrint(ptr.StandardMacros());
        public void Print<T>(IRuntimeProperty<T> prop) => Print(prop.Pointer);

        public void Print(params object[] args)
        {
            var text = new FormattedText();

            foreach (var i in args)
            {
                if (i is string str) text.Text(str);
                else if (i is ScoreRef score) text.Score(score);
                else if (i is ScoreRefOperation op)
                {
                    var x = Local();
                    op.Process(x);
                    text.Score(x);
                }
                else throw new ArgumentException($"Invalid print object {i}. Try using Print<T> for objects on the heap");

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

        public MCFunction? FindFunction(Delegate func)
        {
            var funcs = MCFunctions.ToList();
            var index = funcs.FindIndex((i) => i.Key.Method.MethodHandle.Equals(func.Method.MethodHandle));
            if (index != -1) return funcs[index].Value;

            var methods = RuntimeMethods.ToList();
            index = methods.FindIndex((i) => i.Key.Method.Name == func.Method.Name && (i.Key.Method.DeclaringType?.SameType(func.Method.DeclaringType) ?? false));
            if (index != -1) return methods[index].Value;

            return null;
        }

        public MCFunction AddFunction(Delegate func)
        {
            return AddFunction(func, new(Namespace, DeclareMCAttribute.Get(func).Path));
        }

        public MCFunction AddFunction(Delegate func, NamespacedID id, bool scoped = false)
        {
            var mcfunc = new MCFunction(id, true);
            MCFunctions[func] = mcfunc;

            if (Settings.VerifyMacros) MacroCheckFunctions[id] = new MCFunction(new(id.Namespace, $"zz_macro_check/{id.Path}"), true);

            if (scoped)
            {
                var cleanup = new MCFunction(new(id.Namespace, $"zz_cleanup/{id.Path}"), true);
                MiscMCFunctions.Add(cleanup);

                List<ScoreRef> scope = [.. RegistersInUse];
                var oldFunc = CurrentTarget;
                var oldCleanup = CurrentTargetCleanup;
                var oldSendToMisc = SendToMisc;
                SendToMisc = false;
                CurrentTarget = mcfunc;
                CurrentTargetCleanup = cleanup;

                func.DynamicInvoke();

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
                SendToMisc = oldSendToMisc;
            }
            else
            {
                FunctionsToProcess.Enqueue(new(func, mcfunc));
            }
            return mcfunc;
        }

        public void AddCommand(Command cmd)
        {
            if (SendToMisc)
            {
                MiscInitCmds.Add(cmd);
                return;
            }

            if (cmd is ReturnCommand) Call(CurrentTargetCleanup);
            else if (cmd is Execute ex && ex.Get<Run>().Command is ReturnCommand)
            {
                var other = ex.Copy();
                other.RemoveAll<Run>();

                if (other.Contains<Execute.Conditional.Subcommand>())
                {
                    var tmpExe = new Execute(cmd.Macro);
                    var tmp = Temp(0, 0, "exe_ret");

                    foreach (var i in ex.GetAll<Execute.Conditional.Subcommand>())
                    {
                        tmpExe.Add((Execute.Subcommand)i.Clone());
                    }

                    tmpExe.Store(tmp);
                    tmpExe.Run(Constant(1).Get());
                    AddCommand(tmpExe);

                    other.RemoveAll<Execute.Conditional.Subcommand>();
                    other.If.Score(tmp, 1);
                    ex.RemoveAll<Execute.Conditional.Subcommand>();
                    ex.If.Score(tmp, 1);
                }

                other.Run(new FunctionCommand(CurrentTargetCleanup));
                AddCommand(other);
            }
            else if (Settings.ErrorChecking)
            {
                if (cmd is Execute exe)
                {
                    if (exe.Get<Run>().Command is FunctionCommand) CurrentTarget.Add(new Scoreboard.Players.Set(ErrorScore.Target, ErrorScore.Score, 1));
                    exe.Store(ErrorScore, false);
                }
                else
                {
                    if (cmd is FunctionCommand) CurrentTarget.Add(new Scoreboard.Players.Set(ErrorScore.Target, ErrorScore.Score, 1));
                    CurrentTarget.Add(new Execute(cmd.Macro).Store(ErrorScore, false).Run(cmd));
                }
                CurrentTarget.Add(new Execute(true).If.Score(ErrorScore, 0).Run(new TellrawCommand(new TargetSelector(TargetType.a), new FormattedText().Text($"Command \"{cmd}\" failed"))));
                return;
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

        public void WithInit(Action func)
        {
            SendToMisc = true;
            var old = currentTarget;
            currentTarget = FindFunction(Main);

            func();
            currentTarget = old;
            SendToMisc = false;
        }

        public void AddStaticObject(IStaticType type) => StaticTypes.Add(type);

        public void RegisterObject<T>() where T : IBaseRuntimeObject
        {
            var attr = RuntimeObjectAttribute.Get<T>();

            foreach (var i in typeof(T).GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public))
            {
                if (i.IsStatic && i.GetCustomAttribute<DeclareMCAttribute>() is not null)
                {
                    ProcessRuntimeObjectMethod(i, attr);
                }
            }
        }

        public MCFunction ProcessRuntimeObjectMethod(MethodInfo method, RuntimeObjectAttribute attr)
        {
            var funcAttr = DeclareMCAttribute.Get(method);
            var func = DelegateUtils.Create(method, null);
            var mcFunc = new MCFunction(new(Namespace, $"{attr.Name}/{funcAttr.Path}"), true);
            MCFunctions[func] = mcFunc;
            RuntimeMethods[func] = mcFunc;

            if (Settings.VerifyMacros) MacroCheckFunctions[mcFunc.ID] = new MCFunction(new(mcFunc.ID.Namespace, $"zz_macro_check/{mcFunc.ID.Path}"), true);

            FunctionsToProcess.Enqueue(new(func, mcFunc));
            return mcFunc;
        }

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

        public ScoreRef GetRegister(int index)
        {
            var score = new Score($"_cl_reg_{index}", "dummy");
            Registers.Add(score);
            Scores.Add(score);

            return new ScoreRef(score, GlobalScoreEntity);
        }

        public ScoreRef Local()
        {
            var register = GetRegister(RegistersInUse.Count);
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

        public ScoreRef GetGlobal(int index)
        {
            if (!DuringInit) throw new Exception("Tried to create a global variable outside of Init");

            var score = new Score($"_cl_{Namespace}_var_{index}", "dummy");
            Globals.Add(score);
            Scores.Add(score);

            return new ScoreRef(score, GlobalScoreEntity);
        }

        public ScoreRef Global() => GetGlobal(Globals.Count);

        public ScoreRef Global(int val)
        {
            var global = Global();
            MiscInitCmds.Add(new Scoreboard.Players.Set(global.Target, global.Score, val));
            return global;
        }

        public ScoreRef Constant(int val)
        {
            if (Constants.TryGetValue(val, out var obj)) return obj;

            var score = new Score("_cl_const", "dummy");
            var c = new ScoreRef(score, new NamedTarget($"#_cl_{val}".Replace('-', '_')));

            Scores.Add(score);
            Constants[val] = c;

            return c;
        }

        public FunctionCommand Lambda(Action func) => LambdaWith(AddFunction(func, new(Namespace, $"zz_anon/{AnonymousFuncCounter++}"), true));

        public FunctionCommand LambdaWith(MCFunction mcfunc)
        {
            if (CurrentFunctionAttrs is null || CurrentFunctionAttrs.Macros.Length == 0) return new(mcfunc);

            var nbt = new NBTCompound();

            foreach (var i in CurrentFunctionAttrs.Macros)
            {
                nbt[i] = $"$({i})";
            }

            return new(mcfunc, nbt, true);
        }

        public KeyValuePair<string, object>[] PropagateMacros()
        {
            var args = new List<KeyValuePair<string, object>>();
            foreach (var i in CurrentFunctionAttrs.Macros)
            {
                args.Add(new(i, $"$({i})"));
            }
            return args.ToArray();
        }

        public T AllocObj<T>(bool rtp = true) where T : IBaseRuntimeObject => AllocObj<T>(Local(), rtp);
        public T AllocObj<T>(ScoreRef loc, bool rtp = true, bool cleanup = true) where T : IBaseRuntimeObject
        {
            var ptr = (IPointer<T>)(rtp ? Heap.Alloc(AllocObj<RuntimePointer<T>>(loc, false, false)) : Heap.Alloc<T>(loc));
            var obj = (T)T.Create(ptr);
            if (obj.HasMethod("init")) CallArg(obj.GetMethod("init"), [obj]);

            if (rtp && cleanup) WithCleanup(() => ((RuntimePointer<T>)ptr).FreeObj());

            return obj;
        }

        public T AttachObj<T>(ScoreRef loc, bool rtp = true) where T : IBaseRuntimeObject
        {
            if (!rtp) return (T)T.Create(Attach<T>(loc));
            return (T)T.Create(AttachObj<RuntimePointer<T>>(loc, false));
        }

        public T AllocObjIfNull<T>(ScoreRef loc, bool rtp = true) where T : IBaseRuntimeObject
        {
            var obj = AttachObj<T>(loc, rtp);
            If(!obj.GetPointer().GetHeapPointer().Exists(), () => AllocObj<T>(loc, rtp, false));
            return obj;
        }

        public T GlobalAllocObjIfNull<T>(bool rtp = true) where T : IBaseRuntimeObject
        {
            T? ret = default;
            WithInit(() => ret = AllocObjIfNull<T>(Global(), rtp));
            return ret ?? throw new Exception();
        }

        public HeapPointer<T> Alloc<T>() => Alloc<T>(Local());
        public HeapPointer<T> Alloc<T>(ScoreRef loc) => Heap.Alloc<T>(loc);

        public HeapPointer<T> Alloc<T>(ScoreRef loc, T val) where T : NBTType
        {
            var pointer = Heap.Alloc<T>(loc);
            pointer.Set(val);
            return pointer;
        }

        public HeapPointer<T> Attach<T>(ScoreRef loc) => new(Heap, loc);

        public HeapPointer<T> AllocIfNull<T>(ScoreRef loc, T defaultValue) where T : NBTType
        {
            var ptr = Attach<T>(loc);
            If(!ptr.Exists(), () => Alloc(loc, defaultValue));
            return ptr;
        }

        public IfHandler If(Conditional comp, Action res) => new(this, comp, res);
        public void If(Conditional comp, Command res) => AddCommand(comp.Process(new Execute()).Run(res));

        public void While(Conditional comp, Action res)
        {
            WhileTrue(() =>
            {
                If(!comp, new ReturnCommand(1));
                res();
            });
        }

        public void WhileTrue(Action res)
        {
            var func = Lambda(() =>
            {
                res();
                AddCommand(LambdaWith(CurrentTarget));
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

        public void CallDynamicFunction(Action<MCFunction> func) => Call(ProcessDynamicFunction(func));

        public MCFunction ProcessDynamicFunction(Action<MCFunction> func)
        {
            var mcfunc = new MCFunction(new(Namespace, $"zz_anon/{AnonymousFuncCounter++}"), true);
            func(mcfunc);
            foreach (var i in DynamicFunctions)
            {
                if (i.SameContents(mcfunc))
                {
                    AnonymousFuncCounter--;
                    return i;
                }
            }
            DynamicFunctions.Add(mcfunc);
            return mcfunc;
        }

        public void Strcat(IPointer<string> dest, params object[] values) => Strcat(dest, false, values);

        public void Strcat(IPointer<string> dest, bool macro = false, params object[] values)
        {
            List<KeyValuePair<string, object>> args = [];

            for (int i = 0; i < values.Length; i++)
            {
                args.Add(new(i.ToString(), values[i]));
            }

            Call(ProcessDynamicFunction((func) =>
            {
                var builder = new StringBuilder();

                for (int i = 0; i < values.Length; i++)
                {
                    builder.Append($"$({i})");
                }

                func.Add(new DataCommand.Modify(new StorageMacro("$(storage)"), "$(path).$(pointer)$(ext)", true).Set().Value(builder.ToString()));
            }), [..dest.StandardMacros(), ..args], macro);
        }

        public void DebugLastFunctionCall()
        {
            AddCommand(new DataCommand.Modify(InternalStorage, "test").Set().From(InternalStorage, "func_tmp0"));
            Print($"Debugging for function available with storage {InternalStorage} test");
        }

        public Entity EntityRef(IEntityTarget sel) => EntityRef(sel, Local());

        public Entity EntityRef(IEntityTarget sel, ScoreRef var)
        {
            var id = new ScoreRef(EntityIDScore, sel);
            If(!id.Exists(), () => Std.UniqueEntityID(id));
            var.Set(id);
            return new(var);
        }

        protected virtual void Init() { }

        [DeclareMC("_main")]
        protected virtual void Main() { }

        [DeclareMC("_tick")]
        protected virtual void Tick() { }
    }
}
