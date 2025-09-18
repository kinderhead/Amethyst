using Amethyst.Antlr;
using Amethyst.AST;
using Amethyst.AST.Intrinsics;
using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.InlineFunctions;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Antlr4.Runtime;
using Datapack.Net.Utils;
using GlobExpressions;

namespace Amethyst
{
	public class Compiler : IFileHandler
	{
		public readonly Options Options;
		public readonly GeodeBuilder IR;
		public readonly Dictionary<string, RootNode> Roots = [];
		public readonly Dictionary<NamespacedID, GlobalSymbol> Symbols = [];

		public readonly FunctionContext GlobalInitFunc;

		public Dictionary<string, string> Files { get; } = [];

		public Compiler(Options opts)
		{
			Options = opts;
			IR = new(Options);

			GlobalInitFunc = GetGlobalInitFunc();
			RegisterGlobals();
		}

		public Compiler(string[] args)
		{
			var res = CommandLine.Parser.Default.ParseArguments<Options>(args);
			if (res.Errors.Any()) Environment.Exit(1);
			Options = res.Value;
			Options.Output ??= Path.GetFileName(Options.Inputs.First()) + ".zip";
			IR = new(Options);

			GlobalInitFunc = GetGlobalInitFunc();
			RegisterGlobals();
		}

		public bool Compile()
		{
			var errored = false;
			foreach (var i in new List<string>([.. Options.Inputs.SelectMany(i => Glob.Files(".", i)), .. Glob.Files(Path.Join(AppContext.BaseDirectory, "core"), "**/*.ame").Select(i => Path.Join(AppContext.BaseDirectory, "core", i))]))
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

			if (GlobalInitFunc.FirstBlock.Instructions.Count != 0)
			{
				GlobalInitFunc.Add(new ReturnInsn());
				GlobalInitFunc.Finish();
				IR.AddFunctions(GlobalInitFunc);
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

			var error = new ParserErrorHandler(filename, file, visitor);
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

        public bool WrapError(LocationRange loc, FunctionContext ctx, Action cb)
        {
			ctx.LocationStack.Push(loc);

            try
            {
                cb();
            }
            catch (AmethystError e)
            {
                e.Display(this, loc);
                return false;
            }
			finally
			{
				ctx.LocationStack.Pop();
			}

            return true;
        }

        public void AddSymbol(GlobalSymbol sym)
		{
			if (Symbols.TryGetValue(sym.ID, out var old)) throw new RedefinedSymbolError(sym.ID.ToString(), old.Location);
			else Symbols[sym.ID] = sym;
		}

		public (FunctionContext ctx, FunctionValue func) AnonymousFunction(FunctionTypeSpecifier type)
		{
			var func = new FunctionValue(new("amethyst", "zz_internal/" + GeodeBuilder.RandomString), type);
			var ctx = new FunctionContext(this, func, []);
			IR.AddFunctions(ctx);
			return (ctx, func);
		}

		protected virtual void RegisterGlobals()
		{
			Register(new Print());
			Register(new CountOf());

			AddSymbol(new("builtin:true", LocationRange.None, new LiteralValue(true)));
			AddSymbol(new("builtin:false", LocationRange.None, new LiteralValue(false)));

			var listAdd = new ListAdd();
			AddSymbol(new(listAdd.ID, LocationRange.None, listAdd));
		}

		protected FunctionContext GetGlobalInitFunc() => new(this, new(new("amethyst", "zz_internal/" + GeodeBuilder.RandomString), FunctionTypeSpecifier.VoidFunc), ["minecraft:load"], hasTagPriority: true);

		public void Register(Intrinsic func) => Symbols[func.ID] = new(func.ID, LocationRange.None, func);

		//public static readonly Score ReturnScore = new("returned", "dummy");
	}

	public interface IFileHandler
	{
		public Dictionary<string, string> Files { get; }
	}
}
