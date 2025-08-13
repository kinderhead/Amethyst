using System.Reflection;
using Amethyst.Antlr;
using Amethyst.AST;
using Amethyst.Errors;
using Amethyst.Geode;
using Antlr4.Runtime;
using Datapack.Net.Utils;

namespace Amethyst
{
	public class Compiler : IFileHandler
	{
		public readonly Options Options;
		public readonly GeodeBuilder IR;
		public readonly Dictionary<string, RootNode> Roots = [];
		public readonly Dictionary<NamespacedID, GlobalSymbol> Symbols = [];

		public Dictionary<string, string> Files { get; } = [];

		public Compiler(Options opts)
		{
			Options = opts;
			IR = new(Options);
		}

		public Compiler(string[] args)
		{
			var res = CommandLine.Parser.Default.ParseArguments<Options>(args);
			if (res.Errors.Any()) Environment.Exit(1);
			Options = res.Value;
			Options.Output ??= Path.GetFileName(Options.Inputs.First()) + ".zip";
			IR = new(Options);
		}

		public bool Compile()
		{
			var errored = false;
			foreach (var i in new List<string>([.. Options.Inputs, .. CoreLibrary.Select(i => Path.Join(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location), "core", i))]))
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
				if (!i.Value.CompileFunctions(out var funcs)) errored = true;
				else IR.AddFunctions(funcs);
			}

			if (errored) return false;

			return IR.Compile();
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
				var root = parser.root();
				if (error.Errored) return false;

				Roots[filename] = (RootNode)visitor.Visit(root);
			}
			catch
			{
				if (!error.Errored) throw;
			}

			if (error.Errored) return false;

			return true;
		}

		public bool WrapError(LocationRange loc, Action cb)
		{
			try
			{
				cb();
			}
			catch (AmethystError e)
			{
				e.Display(this, loc);
				return false;
			}

			return true;
		}

		public static readonly List<string> CoreLibrary = ["macros.ame"];
		//public static readonly Score ReturnScore = new("returned", "dummy");
	}

	public interface IFileHandler
	{
		public Dictionary<string, string> Files { get; }
	}
}
