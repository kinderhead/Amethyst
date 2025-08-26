using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Passes;
using Datapack.Net;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;

namespace Amethyst.Geode
{
    public class GeodeBuilder
    {
        public readonly Options Options;
        public readonly DP Datapack;

        public readonly List<FunctionContext> Functions = [];

        public readonly List<Command> UserInitCommands = [];

        private readonly HashSet<Score> registeredScores = [];
        private readonly Dictionary<int, ScoreValue> constants = [];
        private readonly List<MCFunction> functionsToRemove = [];

        public GeodeBuilder(Options opts)
        {
            Options = opts;
            Datapack = GetDP(Options);
        }

        public void AddFunctions(params IEnumerable<FunctionContext> funcs) => Functions.AddRange(funcs);

        public bool Compile()
        {
            ApplyPass<ResolvePass>();

            if (Options.DumpIR)
            {
                DumpIR();
            }

            AllocateRegisters();

            if (Options.DumpIR)
            {
                DumpIR();
                return true;
            }

            foreach (var i in Functions)
            {
                i.Render(this);
            }

            foreach (var i in functionsToRemove)
            {
                Datapack.Functions.Remove(i);
            }

            var init = GetInitFunc();
            Register(init);
            Datapack.Tags.GetTag(new("minecraft", "load"), "function").Values.Insert(0, init.ID);

            var cleanup = GetCleanupFunc();
            Register(cleanup);
            Datapack.Tags.GetTag(new("amethyst", "cleanup"), "function").Values.Insert(0, cleanup.ID);

            Datapack.Optimize();
            Datapack.Build();

            return true;
        }

        public T ApplyPass<T>() where T : Pass, new() => ApplyPass(new T());
        public T ApplyPass<T>(T pass) where T : Pass
        {
            foreach (var i in Functions)
            {
                pass.Apply(i);
            }

            return pass;
        }

        public void AllocateRegisters()
        {
            var inout = ApplyPass<InOutPass>();
            var graph = ApplyPass(new LifetimePass(inout));

            foreach (var func in Functions)
            {
                func.AllocateRegisters(this, graph.Graphs[func]);
            }
        }

        public void DumpIR()
        {
            foreach (var i in Functions)
            {
                Console.WriteLine(i.Dump() + '\n');
            }
        }

        public ScoreValue Score(string name)
        {
            var score = new Score(name, "dummy");
            Register(score);
            return new(RuntimeEntity, score);
        }

        public ScoreValue Reg(int num) => Score($"reg_{num}");
        //public ScoreValue Temp(int num) => Score($"tmp_{num}");
        public ScoreValue Constant(int num)
        {
            if (!constants.TryGetValue(num, out var score)) constants[num] = score = Score($"_{num}");
            return score;
        }

        public void Register(MCFunction func) => Datapack.Functions.Add(func);
        public void Register(Score score) => registeredScores.Add(score);

        public void Unregister(MCFunction func) => functionsToRemove.Add(func);

        private MCFunction GetInitFunc()
        {
            var func = new MCFunction(new("amethyst", $"zz_internal/{RandomString}"));

            func.Add(new DataCommand.Modify(new Storage(new("amethyst", "runtime")), "stack").Set().Value("[{}]"));

            foreach (var i in registeredScores)
            {
                func.Add(new Scoreboard.Objectives.Add(i));
            }

            foreach (var i in constants)
            {
                func.Add(new Scoreboard.Players.Set(i.Value.Target, i.Value.Score, i.Key));
            }

            func.Add([.. UserInitCommands]);

            return func;
        }

        private MCFunction GetCleanupFunc()
        {
            var func = new MCFunction(new("amethyst", $"zz_internal/{RandomString}"));

            foreach (var i in RuntimeStorageUsed)
            {
                func.Add(new DataCommand.Remove(new Storage(new("amethyst", "runtime")), i));
            }

            foreach (var i in registeredScores)
            {
                func.Add(new Scoreboard.Objectives.Remove(i));
            }

            return func;
        }

        //private static readonly Random Random = new(); new ([.. Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz1234567890", 10).Select(s => s[Random.Next(s.Length)])]);

        public static string RandomString { get => Guid.NewGuid().ToString(); }
        public static readonly NamespacedID RuntimeID = new("amethyst", "runtime");
        public static readonly IEntityTarget RuntimeEntity = new NamedTarget("amethyst");
        public static readonly string[] RuntimeStorageUsed = ["stack"];

        private static DP GetDP(Options opts) => new("Project generated with Amethyst", opts.Output, opts.PackFormat);
    }
}
