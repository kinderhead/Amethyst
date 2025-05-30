using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.CubeLib.Builtins.Static;
using Datapack.Net.CubeLib.EntityWrappers;
using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Pack;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private List<IPointer> CleanupPtrs = [];
        public DeclareMCAttribute CurrentFunctionAttrs { get; private set; }

        private List<ScoreRef> RegistersInUse = [];

        public readonly NamedTarget ScoreEntity;
        public readonly Storage InternalStorage;

        public MCStaticStack RegisterStack;
        public MCStaticStack ArgumentStack;
        public MCStaticStack StorageArgumentStack;
        public MCStaticHeap Heap;

        private int AnonymousFuncCounter = 0;

        public int UniqueVars { get; private set; }

        public readonly List<Project> Dependencies = [];

        private bool BuiltOrBuilding = false;

        public CubeLibStd Std;

        public ScoreRef ErrorScore;

        public readonly List<MCFunction> DynamicFunctions = [];

        private bool SendToMisc = false;

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

            AddFunction(Main);

            foreach (var i in GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if (i.GetCustomAttribute<DeclareMCAttribute>() is DeclareMCAttribute attr && attr.Path != "_main") AddFunction(DelegateUtils.Create(i, this));
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

            while (FunctionsToProcess.TryDequeue(out var i))
            {
                RegistersInUse.Clear();
                CurrentTarget = i.Value;
                CurrentTargetCleanup = new MCFunction(new(i.Value.ID.Namespace, $"zz_cleanup/{i.Value.ID.Path}"), true);
                CurrentFunctionAttrs = DeclareMCAttribute.Get(i.Key);
                CleanupPtrs.Clear();
                MiscMCFunctions.Add(CurrentTargetCleanup);

                List<IPointer> pointers = [];

                if (Settings.VerifyMacros && CurrentFunctionAttrs.Macros.Length > 0)
                {
                    var macroCheck = MacroCheckFunctions[CurrentTarget.ID];
                    MiscMCFunctions.Add(macroCheck);
                    foreach (var macro in CurrentFunctionAttrs.Macros)
                    {
                        macroCheck.Add(new Execute(true).Unless.Data(new StorageMacro("$(storage)"), $"$(path).{macro}").Run(new TellrawCommand(new TargetSelector(TargetType.a),
                            new FormattedText()
                                .Text("Error: ", new() { Color = Color.RED })
                                .Text("Missing macro argument at ", new() { Color = Color.RED, HoverText = new FormattedText().Text($"Function: {CurrentTarget.ID}, Argument: {macro}") })
                                .Text("$(line)", new() { Color = Color.YELLOW, HoverText = new FormattedText().Text("$(code)", new() { Color = Color.GREEN }) })
                        )));
                        macroCheck.Add(new Execute(true).Unless.Data(new StorageMacro("$(storage)"), $"$(path).{macro}").Run(new ReturnCommand()));
                    }
                    macroCheck.Add(new ReturnCommand(1));
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
                            pointers.Add(ptr.Cast(e));
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

                    foreach (var rtp in CleanupPtrs)
                    {
                        if (rtp is IBaseRuntimeObject obj && !obj.Freed) obj.FreeObj();
                        else rtp.Free();
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

            if (FindFunction(func) is MCFunction mcfunc) retFunc = mcfunc;
            else retFunc = AddFunction(func);

            var uniqueMacros = new Dictionary<string, object>();

            foreach (var i in macros)
            {
                uniqueMacros[i.Key] = i.Value;
            }

            if (uniqueMacros.Count != 0 && attr.Macros.Length == 0) throw new ArgumentException("Attempted to call a non macro function with arguments");
            else if (uniqueMacros.Count == 0 && attr.Macros.Length != 0) throw new ArgumentException("Attempted to call a macro function without arguments");
            else if (uniqueMacros.Count != attr.Macros.Length) throw new ArgumentException("Mismatch in number of macro arguments passed to and accepted by function");
            else if (uniqueMacros.Count != 0)
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

        public FunctionCommand BaseCall(MCFunction func, KeyValuePair<string, object>[] args, bool macro = false, int tmp = 0)
        {
            var parameters = new NBTCompound();
            var runtimeScores = new Dictionary<string, ScoreRef>();
            var pointers = new Dictionary<string, IPointer>();

            var newScores = 0;
            foreach (var i in args)
            {
                if (NBTValue.ToNBT(i.Value) != null) parameters[i.Key] = NBTValue.ToNBT(i.Value) ?? throw new Exception("How did we get here?");
                else if (i.Value.GetType().IsAssignableTo(typeof(NBTValue))) parameters[i.Key] = (NBTValue)i.Value;
                else if (i.Value is ScoreRef score) runtimeScores[i.Key] = score;
                else if (i.Value is ScoreRefOperation op)
                {
                    var var = Temp(newScores++, $"exe{tmp}");
                    op.Process(var);
                    runtimeScores[i.Key] = var;
                }
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

            if (Settings.VerifyMacros && !func.ID.Path.StartsWith("zz_"))
            {
                var ret = Temp(0, "call_fail");
                AddCommand(new Execute(macro).Store(ret, false).Run(new FunctionCommand(MacroCheckFunctions[func.ID], new NBTCompound
                {
                    ["storage"] = InternalStorage.ToString(),
                    ["path"] = $"func_tmp{tmp}",
                    ["line"] = GetLineData().Item1,
                    ["code"] = NBTString.Escape(GetLineData().Item2)
                })));
                if (!CurrentTarget.ID.Path.StartsWith("zz_")) If(ret == 0, new ReturnCommand());
            }

            return new FunctionCommand(func, InternalStorage, $"func_tmp{tmp}", macro);
        }

        public FunctionCommand BaseCall(MCFunction func, IRuntimeArgument[] args, bool macro = false)
        {
            PushArgs(args);
            return new FunctionCommand(func, macro);
        }

        public FunctionCommand BaseCall(MCFunction func, IRuntimeArgument[] args, KeyValuePair<string, object>[] macros, bool macro = false, int tmp = 0)
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

                    // :)
                    dynamic ptr = rawPtr.GetType().GetMethod("ToRTP", 1, [])?.MakeGenericMethod(obj.GetType()).Invoke(rawPtr, []) ?? throw new Exception();

                    StorageArgumentStack.Enqueue(ptr.SelfPointer);
                }
                else ArgumentStack.Enqueue(i.GetAsArg());
            }
        }

        public void Print<T>(IPointer<T> ptr) where T : IPointerable => Std.PointerPrint(ptr.StandardMacros());
        public void Print<T>(RuntimePointer<T> ptr) where T : IPointerable => Print((IPointer<T>)ptr);
        public void Print<T>(IRuntimeProperty<T> prop) where T : IPointerable => Print(prop.Pointer);
        public void Print<T>(EntityProperty<T> prop) where T : NBTValue => Print(prop.ToPointer());

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

            if (func.Target != this && func.Target is Project lib)
            {
                if (lib.BuiltOrBuilding) return lib.FindFunction(func);
                return new(new(lib.Namespace, DeclareMCAttribute.Get(func).Path), true);
            }

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
                var oldPtrs = CleanupPtrs;
                SendToMisc = false;
                CurrentTarget = mcfunc;
                CurrentTargetCleanup = cleanup;
                CleanupPtrs = [];

                func.DynamicInvoke();

                WithCleanup(() =>
                {
                    foreach (var rtp in CleanupPtrs)
                    {
                        if (rtp is IBaseRuntimeObject obj && !obj.Freed) obj.FreeObj();
                        else rtp.Free();
                    }

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
                CleanupPtrs = oldPtrs;
            }
            else
            {
                FunctionsToProcess.Enqueue(new(func, mcfunc));
            }
            return mcfunc;
        }

        private string lastLine;
        public void AddCommand(Command cmd)
        {
            if (SendToMisc)
            {
                MiscInitCmds.Add(cmd);
                return;
            }

            var line = GetCallingLine();

            if (line != lastLine)
            {
                lastLine = line;
                CurrentTarget.Add(new Comment($" {line}"));
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

        public T WithInit<T>(Func<T> func)
        {
            SendToMisc = true;
            var old = currentTarget;
            currentTarget = FindFunction(Main);

            var ret = func();
            currentTarget = old;
            SendToMisc = false;
            return ret;
        }

        public void WithInit(Action func)
        {
            WithInit(() =>
            {
                func();
                return false;
            });
        }

        public void AddStaticObject(IStaticType type) => StaticTypes.Add(type);

        public void RegisterObject<T>() where T : IBaseRuntimeObject
        {
            var attr = RuntimeObjectAttribute.Get<T>();

            foreach (var i in typeof(T).GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public))
            {
                if (i.IsStatic && i.GetCustomAttribute<DeclareMCAttribute>() is DeclareMCAttribute m && (Settings.ReferenceChecking || m.Path != "deinit"))
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
            var score = new Score($"_cl_{Namespace}_var_{index}", "dummy");
            Globals.Add(score);
            Scores.Add(score);

            return new(score, GlobalScoreEntity, global: true);
        }

        public ScoreRef GetGlobal(string name)
        {
            var score = new Score($"_cl_{Namespace}_var_{name}", "dummy");
            Scores.Add(score);

            return new(score, GlobalScoreEntity, global: true);
        }

        public ScoreRef Global() => GetGlobal(Globals.Count);
        public ScoreRef Global(string name) => GetGlobal(name);

        public ScoreRef Global(int val)
        {
            var global = Global();
            MiscInitCmds.Add(new Scoreboard.Players.Set(global.Target, global.Score, val));
            return global;
        }

        public ScoreRef NewUnique()
        {
            var score = new Score($"_cl_{Namespace}_unique_{UniqueVars++}", "dummy");
            Scores.Add(score);
            return new(score, GlobalScoreEntity, global: true);
        }

        public ScoreRef Constant(int val)
        {
            if (Constants.TryGetValue(val, out var obj)) return obj;

            var score = new Score("_cl_const", "dummy");
            var c = new ScoreRef(score, new NamedTarget($"#_cl_{val}".Replace('-', '_')), global: true);

            Scores.Add(score);
            Constants[val] = c;

            return c;
        }

        public FunctionCommand Lambda(Action func) => Lambda(AddFunction(func, new(Namespace, $"zz_anon/{AnonymousFuncCounter++}"), true));

        public FunctionCommand Lambda(MCFunction mcfunc)
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
            return [.. args];
        }

        public T AllocObj<T>(bool rtp = true) where T : IBaseRuntimeObject => AllocObj<T>(Local(), rtp);
        public T AllocObj<T>(ScoreRef loc, bool rtp = true, bool cleanup = true) where T : IBaseRuntimeObject
        {
            var ptr = (IPointer<T>)(rtp ? Heap.Alloc(AllocObj<RuntimePointer<T>>(loc, false, false)) : Heap.Alloc<T>(loc));
            var obj = T.Create(ptr);
            if (obj.HasMethod("init")) CallArg(obj.GetMethod("init"), [obj]);

            if (rtp && Settings.ReferenceChecking)
            {
                obj.ReferenceCount.Pointer.Set((NBTInt)1);
            }

            if (!loc.Global && rtp && cleanup) CleanupPtrs.Add(ptr);

            return obj;
        }

        public T AttachObj<T>(ScoreRef loc, bool rtp = true) where T : IBaseRuntimeObject
        {
            if (!rtp) return T.Create(Attach<T>(loc));
            return T.Create(AttachObj<RuntimePointer<T>>(loc, false));
        }

        public T AllocObjIfNull<T>(ScoreRef loc, bool rtp = true) where T : IBaseRuntimeObject
        {
            var obj = AttachObj<T>(loc, rtp);
            If(!obj.GetPointer().GetHeapPointer().Exists(), () => AllocObj<T>(loc, rtp, false));
            return obj;
        }

        public HeapPointer<T> Alloc<T>(bool free = true) where T : NBTValue => Alloc<T>(Local(), free);
        public HeapPointer<T> Alloc<T>(T val, bool free = true) where T : NBTValue => Alloc(Local(), val, free);
        public HeapPointer<T> Alloc<T>(ScoreRef loc, bool free = true) where T : NBTValue
        {
            var pointer = Heap.Alloc<T>(loc);

            if (loc.Global) Console.WriteLine("Warning: allocating using a global variable without AllocIfNull<T>()");

            if (!loc.Global && free) WithCleanup(pointer.Free);
            return pointer;
        }

        public HeapPointer<T> Alloc<T>(ScoreRef loc, T val, bool free = true) where T : NBTValue
        {
            var pointer = Alloc<T>(loc, free);
            pointer.Set(val);
            return pointer;
        }

        public HeapPointer<T> Attach<T>(ScoreRef loc) where T : IPointerable => new(Heap, loc);

        public HeapPointer<T> AllocIfNull<T>(ScoreRef loc, T defaultValue) where T : NBTValue
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
                AddCommand(Lambda(CurrentTarget));
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

        public void As(IEntityTarget targets, Action<Entity> func)
        {
            Entity.AsStack.Push(null);
            AddCommand(new Execute().As(targets).Run(Lambda(() =>
            {
                var self = EntityRef(TargetSelector.Self);
                func(self);
            })));
            Entity.AsStack.Pop();
        }

        public Entity Summon(EntityType type) => Summon(type, Position.Current);
        public Entity Summon(EntityType type, Position pos) => Summon(Local(), type, pos);

        public Entity Summon(ScoreRef loc, EntityType type, Position pos)
        {
            AddCommand(new Execute().Positioned(pos).Summon(type).Store(loc).Run(new FunctionCommand(Std.GetEntityID_Function())));
            return new(loc);
        }

        public T Summon<T>(ScoreRef loc, Position pos) where T : EntityWrapper
        {
            var entity = (T?)Activator.CreateInstance(typeof(T), [loc]) ?? throw new ArgumentException("Failed to create entity");
            Summon(loc, entity.Type, pos);
            return entity;
        }

        public T Summon<T>(Position pos) where T : EntityWrapper => Summon<T>(Local(), pos);
        public T Summon<T>() where T : EntityWrapper => Summon<T>(Local(), Position.Current);

        public Entity SummonIfDead(ScoreRef loc, EntityType type, Position pos)
        {
            var e = new Entity(loc);
            If(!e.Exists(), () => Summon(loc, type, pos));
            return e;
        }

        public Entity SummonIfDead(ScoreRef loc, EntityType type) => SummonIfDead(loc, type, Position.Current);
        public T SummonIfDead<T>(ScoreRef loc) where T : EntityWrapper => SummonIfDead<T>(loc, Position.Current);

        public T SummonIfDead<T>(ScoreRef loc, Position pos) where T : EntityWrapper
        {
            var entity = (T?)Activator.CreateInstance(typeof(T), [loc]) ?? throw new ArgumentException("Failed to create entity");
            SummonIfDead(loc, entity.Type, pos);
            return entity;
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

        public void Strcat(IPointer<NBTString> dest, params object[] values) => Strcat(dest, false, values);

        public void Strcat(IPointer<NBTString> dest, bool macro = false, params object[] values)
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
            }), [.. dest.StandardMacros(), .. args], macro);
        }

        public void DebugLastFunctionCall(int tmp = 0)
        {
            AddCommand(new DataCommand.Modify(InternalStorage, "test").Set().From(InternalStorage, $"func_tmp{tmp}"));
            Print($"Debugging for function available with storage {InternalStorage} test");
        }

        public (string, string) GetLineData()
        {
            var trace = new StackTrace(1, true);

            foreach (var i in trace.GetFrames())
            {
                if (i.GetMethod()?.DeclaringType?.Namespace?.StartsWith("Datapack.Net") == true) continue;
                else if (i.GetMethod()?.DeclaringType?.BaseType != typeof(Project)) continue;
                else if (i.GetFileName() is null) continue;
                else return ($"{Path.GetFileName(i.GetFileName())}:{i.GetFileLineNumber()}", File.ReadLines(i.GetFileName() ?? throw new Exception("Couldn't read file")).Skip(i.GetFileLineNumber() - 1).Take(1).First().Trim());
            }

            return ("<builtin>", "<builtin>");
        }

        public string GetCallingLine()
        {
            var trace = new StackTrace(3, true);
            StackFrame? frame = null;

            foreach (var i in trace.GetFrames())
            {
                if (i.GetMethod()?.DeclaringType != typeof(Project) && i.GetFileName()?.EndsWith(".g.cs") == false)
                {
                    frame = i;
                    break;
                }
            }

            if (frame is null) return "<builtin>";
            return $"{Path.GetFileName(frame.GetFileName())}:{frame.GetFileLineNumber()}";
        }

        public Entity EntityRef(IEntityTarget sel) => EntityRef(sel, Local());
        public Entity GlobalEntityRef(IEntityTarget sel) => EntityRef(sel, Global());

        public Entity EntityRef(IEntityTarget sel, ScoreRef var)
        {
            AddCommand(new Execute().As(sel.RequireOne()).Store(var).Run(new FunctionCommand(Std.GetEntityID_Function())));

            return new(var);
        }

        public T EntityRef<T>(TargetSelector sel, ScoreRef var) where T : EntityWrapper
        {
            var entity = EntityWrapper.Create<T>(EntityRef(sel, var));
            if (sel.Type?.First()?.Value != entity.Type) throw new ArgumentException("Entity target selector type is invalid");
            return entity;
        }

        public T EntityRef<T>(TargetSelector sel) where T : EntityWrapper => EntityRef<T>(sel, Local());

        public void Async(Action func, int ticks) => Async(GuaranteeFunc(func, []), ticks);
        public void Async(MCFunction func, int ticks) => AddCommand(new ScheduleCommand(func, ticks));

        [DeclareMC("_main")]
        protected virtual void Main() { }

        [DeclareMC("_tick")]
        protected virtual void Tick() { }
    }
}
