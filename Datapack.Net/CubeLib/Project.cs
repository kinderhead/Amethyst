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
    public abstract class Project(string ns)
    {
        public static Project ActiveProject { get; private set; }

        public readonly Datapack Datapack = new();
        public readonly string Namespace = ns;

        private readonly Dictionary<Action, MCFunction> MCFunctions = [];
        private readonly Queue<KeyValuePair<Action, MCFunction>> FunctionsToProcess = [];

        private readonly HashSet<Score> Scores = [];
        private readonly HashSet<ScoreRef> Registers = [];

        public MCFunction? CurrentTarget { get; private set; }
        protected readonly List<ScoreRef> RegistersInUse = [];

        public readonly NamedTarget ScoreEntity = new($"_{ns}_cubelib_score");
        public readonly Storage InternalStorage = new(new(ns, "_internal"));

        public void Build()
        {
            ActiveProject = this;

            Init();

            AddFunction(Main);
            AddFunction(Tick);

            while (FunctionsToProcess.TryDequeue(out var i))
            {
                CurrentTarget = i.Value;
                i.Key();
            }

            // Prepend Main
            foreach (var i in Scores.Reverse())
            {
                MCFunctions[Main].Prepend(new Scoreboard.Objectives.Add(i));
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
            AddCommand(new SayCommand(msg));
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

        public void AddScore(Score score) => Scores.Add(score);

        public ScoreRef Temp(int num, string type = "tmp")
        {
            var score = new Score($"_cl_{type}_{num}", "dummy");
            if (!Scores.Contains(score)) AddScore(score);
            return new(score, ScoreEntity);
        }

        protected virtual void Init() { }

        [DeclareMC("main")]
        protected virtual void Main() { }

        [DeclareMC("tick")]
        protected virtual void Tick() { }
    }
}
