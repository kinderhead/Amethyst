using Datapack.Net;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Pack;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Passes;
using Geode.Types;
using Geode.Values;

namespace Geode
{
	public class GeodeBuilder
	{
		public readonly IOptions Options;
		public readonly ICompiler Compiler;
		public readonly IFileHandler FileHandler;
		public readonly DP Datapack;
		public readonly Macroizer Macroizer;

		public readonly string Namespace;
		public readonly NamespacedID RuntimeID;
		public readonly IEntityTarget RuntimeEntity;

		public readonly Dictionary<NamespacedID, GlobalSymbol> Symbols = [];
		public readonly Dictionary<NamespacedID, GlobalTypeSymbol> Types = [];

		public readonly List<FunctionContext> Functions = [];

		public readonly List<Command> UserInitCommands = [];

		private readonly SortedSet<Score> registeredScores = [];
		private readonly SortedDictionary<int, ScoreValue> constants = [];
		private readonly List<MCFunction> functionsToRemove = [];

		public GeodeBuilder(IOptions opts, ICompiler compiler, IFileHandler handler, string baseNamespace)
		{
			Namespace = baseNamespace;
			RuntimeID = new(baseNamespace, "runtime");
			RuntimeEntity = new NamedTarget(baseNamespace);

			Options = opts;
			Datapack = GetDP(Options);
			Compiler = compiler;
			FileHandler = handler;
			Macroizer = new(this);
		}

		public void AddFunctions(params IEnumerable<FunctionContext> funcs) => Functions.AddRange(funcs);

		private bool failed = false;
		public bool Compile()
		{
			failed = false;

			ApplyPass<InlinePass>();
			ApplyPass<ResolvePass>();

			if (failed)
			{
				return false;
			}

			if (Options.DumpIR)
			{
				DumpIR();
			}

			AllocateRegisters();

			if (failed)
			{
				return false;
			}

			if (Options.DumpIR)
			{
				DumpIR();
				return true;
			}

			foreach (var i in Functions)
			{
				try
				{
					i.Render(this);
				}
				catch (EmptyGeodeError)
				{
					failed = true;
				}
			}

			if (failed)
			{
				return false;
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
			if (pass.MinimumOptimizationLevel > Options.OptimizationLevel)
            {
                return pass;
            }

			foreach (var i in Functions)
			{
				try
				{
					pass.Apply(i);
				}
				catch (EmptyGeodeError)
				{
					failed = true;
				}
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
				if (!i.Decl.ID.ToString().StartsWith("amethyst:core"))
				{
					Console.WriteLine(i.Dump() + '\n');
				}
			}
		}

		public ScoreValue Score(string name)
		{
			var score = new Score(name, "dummy");
			Register(score);
			return new(RuntimeEntity, score);
		}

		public ScoreValue Reg(int num) => Score($"reg_{num}");
		public ScoreValue Temp(int num) => Score($"tmp_{num}");

		public ScoreValue Constant(int num)
		{
			if (!constants.TryGetValue(num, out var score))
			{
				constants[num] = score = Score($"_{num}");
			}

			return score;
		}

		public StorageValue TempStorage(TypeSpecifier type) => new(RuntimeID, $"tmp.{RandomString}", type);

		public void AddSymbol(GlobalSymbol sym)
		{
			if (Symbols.TryGetValue(sym.ID, out var old))
			{
				throw new RedefinedSymbolError(sym.ID.ToString(), old.Location);
			}
			else
			{
				Symbols[sym.ID] = sym;
			}
		}

		public void AddType(GlobalTypeSymbol sym)
		{
			if (Types.TryGetValue(sym.ID, out var old))
			{
				throw new RedefinedSymbolError(sym.ID.ToString(), old.Location);
			}
			else
			{
				Types[sym.ID] = sym;
			}
		}

		public (FunctionContext ctx, FunctionValue func) AnonymousFunction(FunctionType type)
		{
			var func = new FunctionValue(new("amethyst", InternalPath + "/" + RandomString), type);
			var ctx = new FunctionContext(Compiler, func, [], LocationRange.None);
			AddFunctions(ctx);
			return (ctx, func);
		}

		public IValue? GetGlobal(NamespacedID id)
		{
			if (Symbols.TryGetValue(id, out var sym))
			{
				return sym.Value;
			}

			return null;
		}

		public IValue? GetGlobalWalk(string baseNamespace, string name) => NamespaceWalk(baseNamespace, name, Symbols)?.Value;

		public IValue? GetConstructorOrNull(TypeSpecifier type)
		{
			if (GetGlobal(type.ID) is IValue v && v.Type is FunctionType funcType && funcType.ReturnType == type)
			{
				return v;
			}

			return null;
		}

		public void Register(MCFunction func) => Datapack.Functions.Add(func);
		public void Register(Score score) => registeredScores.Add(score);

		public void Unregister(MCFunction func) => functionsToRemove.Add(func);

		private MCFunction GetInitFunc()
		{
			var func = new MCFunction(new("amethyst", $"{InternalPath}/{RandomString}"));

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
			var func = new MCFunction(new("amethyst", $"{InternalPath}/{RandomString}"));

			foreach (var i in RuntimeStorageUsed)
			{
				func.Add(new DataCommand.Remove(RuntimeID, i));
			}

			foreach (var i in registeredScores)
			{
				func.Add(new Scoreboard.Objectives.Remove(i));
			}

			return func;
		}

		public const string InternalPath = "zz_internal";
		public static string RandomString => Guid.NewGuid().ToString();
		public static readonly string[] RuntimeStorageUsed = ["stack", "tmp"];

		public static T? NamespaceWalk<T>(string baseNamespace, string name, Dictionary<NamespacedID, T> syms)
		{
			if (syms.TryGetValue(new(baseNamespace, name), out var v))
			{
				return v;
			}
			else if (baseNamespace.Contains('/'))
			{
				return NamespaceWalk(baseNamespace[..baseNamespace.LastIndexOf('/')], name, syms);
			}
			else if (baseNamespace.Contains(':'))
			{
				return NamespaceWalk(baseNamespace[..baseNamespace.LastIndexOf(':')], name, syms);
			}
			else
			{
				return default;
			}
		}

		private static DP GetDP(IOptions opts) => new(opts.Output, new MCMeta().SetDescription("A project made with Amethyst"));
	}
}
