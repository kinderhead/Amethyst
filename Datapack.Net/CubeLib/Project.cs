using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Pack;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Datapack.Net.Data._1_20_4.Blocks;

namespace Datapack.Net.CubeLib
{
    public abstract class Project(string ns, Datapack pack)
    {
        public static Project ActiveProject { get; private set; }

        public readonly Datapack Datapack = pack;
        public readonly string Namespace = ns;

        private readonly Dictionary<Action, MCFunction> MCFunctions = [];
        private readonly Queue<KeyValuePair<Action, MCFunction>> FunctionsToProcess = [];

        private readonly HashSet<Score> Scores = [];
        private readonly HashSet<Score> Registers = [];
        private readonly HashSet<Score> Globals = [];
        private readonly List<ICubeLangType> CubeLangTypes = [];
        private readonly List<Command> MiscInitCmds = [];

        public MCFunction? CurrentTarget { get; private set; }
        private List<ScoreRef> RegistersInUse = [];

        public readonly NamedTarget ScoreEntity = new($"_{ns}_cubelib_score");
        public readonly Storage InternalStorage = new(new(ns, "_internal"));

        public MCStack RegisterStack;

        private int AnonymousFuncCounter = 0;

        public void Build()
        {
            ActiveProject = this;

            Init();

            AddFunction(Main);
            AddFunction(Tick);

            var tags = Datapack.GetResource<Tags>();
            var mainTag = new Tag(new("minecraft", "load"), "functions");
            var tickTag = new Tag(new("minecraft", "tick"), "functions");

            mainTag.Values.Add(new(Namespace, "main"));
            tickTag.Values.Add(new(Namespace, "tick"));

            tags.AddTag(mainTag);
            tags.AddTag(tickTag);

            RegisterStack = new(InternalStorage, "register_stack");
            AddObject(RegisterStack);

            while (FunctionsToProcess.TryDequeue(out var i))
            {
                RegistersInUse.Clear();
                CurrentTarget = i.Value;
                i.Key();

                RegistersInUse.Reverse();
                foreach (var r in RegistersInUse)
                {
                    RegisterStack.Dequeue(r);
                }
            }

            // Prepend Main
            foreach (var i in CubeLangTypes)
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

            Datapack.Build();
        }

        public void Call(Action func)
        {
            if (!MCFunctions.ContainsKey(func))
            {
                AddFunction(func);
            }

            AddCommand(new FunctionCommand(MCFunctions[func]));
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

        public MCFunction AddFunction(Action func)
        {
            return AddFunction(func, new(Namespace, DeclareMCAttribute.Get(func).Name));
        }

        public MCFunction AddFunction(Action func, NamespacedID id, bool scoped = false)
        {
            var mcfunc = new MCFunction(id, true);
            MCFunctions[func] = mcfunc;
            if (scoped)
            {
                List<ScoreRef> scope = [.. RegistersInUse];
                var oldFunc = CurrentTarget;
                CurrentTarget = mcfunc;

                func();

                for (int i = RegistersInUse.Count - 1; i >= scope.Count; i--)
                {
                    RegisterStack.Dequeue(RegistersInUse[i]);
                }

                RegistersInUse = scope;
                CurrentTarget = oldFunc;
            }
            else
            {
                FunctionsToProcess.Enqueue(new(func, mcfunc));
            }
            return mcfunc;
        }

        public void AddCommand(Command cmd)
        {
            if (CurrentTarget == null) throw new InvalidOperationException("Project not building yet");
            CurrentTarget.Add(cmd);
        }

        public void AddObject(ICubeLangType type) => CubeLangTypes.Add(type);

        public void PrependMain(Command cmd)
        {
            if (!MCFunctions.ContainsKey(Main)) throw new InvalidOperationException("Project not building yet");
            MCFunctions[Main].Prepend(cmd);
        }

        public void AddScore(Score score) => Scores.Add(score);

        public ScoreRef Temp(int num, string type = "tmp")
        {
            var score = new Score($"_cl_{type}_{num}", "dummy");
            if (!Scores.Contains(score)) AddScore(score);
            return new(score, ScoreEntity);
        }

        public ScoreRef Temp(int num, int val, string type = "tmp")
        {
            var tmp = Temp(num, type);
            tmp.Set(val);
            return tmp;
        }

        public ScoreRef Local()
        {
            var score = new Score($"_cl_{Namespace}_reg_{RegistersInUse.Count}", "dummy");
            Registers.Add(score);
            Scores.Add(score);

            var register = new ScoreRef(score, ScoreEntity);
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

            return new ScoreRef(score, ScoreEntity);
        }

        public ScoreRef Global(int val)
        {
            var global = Global();
            MiscInitCmds.Add(new Scoreboard.Players.Set(global.Target, global.Score, val));
            return global;
        }

        public MCFunction Lambda(Action func)
        {
            var mcfunc = AddFunction(func, new(Namespace, $"zz_anon/{AnonymousFuncCounter}"), true);
            AnonymousFuncCounter++;

            return mcfunc;
        }

        public void If(ScoreRefComparison comp, Action res)
        {
            var cmd = comp.Process(new Execute());
            AddCommand(cmd.Run(new FunctionCommand(Lambda(res))));
        }

        protected virtual void Init() { }

        [DeclareMC("main")]
        protected virtual void Main() { }

        [DeclareMC("tick")]
        protected virtual void Tick() { }
    }
}
