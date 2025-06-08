using Amethyst.Antlr;
using Amethyst.AST;
using Amethyst.Codegen;
using Amethyst.Codegen.Functions;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Antlr4.Runtime;
using CommandLine;
using Datapack.Net;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst
{
	public class Compiler : IFileHandler
	{
		public readonly Options Options;
		public readonly DP Datapack;
		public readonly Dictionary<string, RootNode> Roots = [];
		public readonly Dictionary<NamespacedID, GlobalSymbol> Symbols = [];
		public readonly Dictionary<NamespacedID, FunctionContext> Functions = [];
		public readonly Dictionary<NamespacedID, CompileTimeFunction> CompileTimeFunctions = [];

		private readonly Dictionary<int, ScoreValue> constants = [];
		private readonly HashSet<Score> registeredScores = [];
		private readonly List<MCFunction> functionsToRemove = [];

		public Dictionary<string, string> Files { get; } = [];

		public Compiler(Options opts)
		{
			Options = opts;
			Datapack = GetDP(Options);
			SetupDefaults();
		}

		public Compiler(string[] args)
		{
			var res = CommandLine.Parser.Default.ParseArguments<Options>(args);
			if (res.Errors.Any()) Environment.Exit(1);
			Options = res.Value;
			Options.Output ??= Path.GetFileName(Options.Inputs.First()) + ".zip";
			Datapack = GetDP(Options);
			SetupDefaults();
		}

		public bool Compile()
		{
			var errored = false;
			foreach (var i in new List<string>([..Options.Inputs, ..CoreLibrary.Select(i => Path.Join(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location), "core", i))]))
			{
				if (!ParseFile(i)) errored = true;
			}

			if (errored) return false;

			// Do class decls before
			foreach (var i in Roots)
			{
				if (!i.Value.BuildSymbols()) errored = true;
			}

			foreach (var i in Roots)
			{
				if (!i.Value.CompileFunctions()) errored = true;
			}

			//// Maybe remove this?
			//if (errored) return false;

			//foreach (var i in Functions)
			//{
			//	if (!i.Value.RequireCompiled()) errored = true;
			//}

			foreach (var i in functionsToRemove)
			{
				Datapack.Functions.Remove(i);
			}

			if (errored) return false;

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

		public void Register(MCFunction func) => Datapack.Functions.Add(func);
		public void Register(Score score) => registeredScores.Add(score);
		public void Register(CompileTimeFunction func) => CompileTimeFunctions[func.ID] = func;

		public void Unregister(MCFunction func) => functionsToRemove.Add(func);

		public ScoreValue Constant(int num)
		{
			if (constants.TryGetValue(num, out var val)) return val;

			var score = new Score($"_{num}", "dummy");
			Register(score);
			val = new ScoreValue(RuntimeEntity, score);
			constants[num] = val;

			return val;
		}

		public bool ParseFile(string filename)
		{
			var file = File.ReadAllText(filename);
			Files[filename] = file;

			var input = new AntlrInputStream(file);
			var lexer = new AmethystLexer(input);
			var tokens = new CommonTokenStream(lexer);
			var parser = new AmethystParser(tokens);
			var visitor = new Visitor(filename, this);

			var error = new ParserErrorHandler(filename, file);
			lexer.RemoveErrorListeners();
			lexer.AddErrorListener(error);
			parser.RemoveErrorListeners();
			parser.AddErrorListener(error);

			try
			{
				Roots[filename] = (RootNode)visitor.Visit(parser.root());
			}
			catch
			{
				if (!error.Errored) throw;
			}

			if (error.Errored) return false;

			return true;
		}

		public bool WrapError(Action cb)
		{
			try
			{
				cb();
			}
			catch (AmethystError e)
			{
				e.Display(this);
				return false;
			}

			return true;
		}

		private MCFunction GetInitFunc()
		{
			var func = new MCFunction(new("amethyst", $"zz_internal/{RandomString}"));

			func.Add(new DataCommand.Modify(new Storage(new("amethyst", "runtime")), "stack").Set().Value("[]"));

			foreach (var i in registeredScores)
			{
				func.Add(new Scoreboard.Objectives.Add(i));
			}

			foreach (var i in constants)
			{
				func.Add(new Scoreboard.Players.Set(i.Value.Target, i.Value.Score, i.Key));
			}

			//func.Add(new Scoreboard.Players.Set(RuntimeEntity, NullScore, 0));

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

		private void SetupDefaults()
		{
			//Register(NullScore);
			Register(new PrintFunction());
		}

		private static DP GetDP(Options opts) => new("Project generated with Amethyst", opts.Output, opts.PackFormat);

		//private static readonly Random Random = new(); new ([.. Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz1234567890", 10).Select(s => s[Random.Next(s.Length)])]);

		public static string RandomString { get => Guid.NewGuid().ToString(); }
		public static readonly NamespacedID RuntimeID = new("amethyst", "runtime");
		public static readonly IEntityTarget RuntimeEntity = new NamedTarget("amethyst");
		public static readonly List<string> RuntimeStorageUsed = ["stack", "return", "tmp_args", "tmp_macros"];
		public static readonly List<string> CoreLibrary = ["macros.ame"];
		//public static readonly Score ReturnScore = new("returned", "dummy");
	}

	public interface IFileHandler
	{
		public Dictionary<string, string> Files { get; }
	}
}
