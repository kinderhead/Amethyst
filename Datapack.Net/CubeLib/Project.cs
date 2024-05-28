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
        private readonly List<ICubeLangType> CubeLangTypes = [];

        public MCFunction? CurrentTarget { get; private set; }
        protected readonly List<ScoreRef> RegistersInUse = [];

        public readonly NamedTarget ScoreEntity = new($"_{ns}_cubelib_score");
        public readonly Storage InternalStorage = new(new(ns, "_internal"));

        public MCStack RegisterStack;

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

        public void Print(string msg)
        {
            AddCommand(new TellrawCommand(new TargetSelector(TargetType.a), new FormattedText().Text(msg)));
        }

        public void Print(ScoreRef val)
        {
            AddCommand(new TellrawCommand(new TargetSelector(TargetType.a), new FormattedText().Score(val)));
        }

        public MCFunction AddFunction(Action func)
        {
            var id = new NamespacedID(Namespace, DeclareMCAttribute.Get(func).Name);
            var mcfunc = new MCFunction(id, true);
            MCFunctions[func] = mcfunc;
            FunctionsToProcess.Enqueue(new(func, mcfunc));
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

        public ScoreRef Local()
        {
            var score = new Score($"_cl_reg_{RegistersInUse.Count}", "dummy");
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

        protected virtual void Init() { }

        [DeclareMC("main")]
        protected virtual void Main() { }

        [DeclareMC("tick")]
        protected virtual void Tick() { }
    }
}
