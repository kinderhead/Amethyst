using Amethyst.Antlr;
using Amethyst.AST;
using Amethyst.AST.Intrinsics;
using Amethyst.Cli;
using Antlr4.Runtime;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;
using GlobExpressions;
using Spectre.Console;
using System.Runtime.InteropServices;

namespace Amethyst
{
	public class Compiler : ICompiler, IFileHandler
	{
		public readonly BuildOptions Options;
		public readonly Dictionary<string, RootNode> Roots = [];
		public readonly FunctionContext GlobalInitFunc;

		public Dictionary<string, string> Files { get; } = [];
		public GeodeBuilder IR { get; }

		public Compiler(BuildOptions opts)
		{
			Options = opts;
			IR = new(Options, this, "amethyst");

			GlobalInitFunc = GetGlobalInitFunc();
			RegisterGlobals();
		}

		public bool Compile()
		{
			var errored = false;

			List<string> inputs = [];

			foreach (var glob in Options.Inputs.Select(i => i.Replace('\\', '/')))
			{
				var lastSlash = 0;
				var firstStar = -1;

				for (var i = 0; i < glob.Length; i++)
				{
					if (glob[i] == '/')
					{
						lastSlash = i;
					}

					if (glob[i] == '*')
					{
						firstStar = i;
						break;
					}
				}

				if (firstStar == -1)
				{
					inputs.Add(glob);
					continue;
				}

				IEnumerable<string> globToCheck;

				if (lastSlash == 0)
				{
					globToCheck = Glob.Files(Directory.GetCurrentDirectory(), glob);
				}
				else
				{
					globToCheck = Glob.Files(glob[..lastSlash], glob[(lastSlash + 1)..]);
				}

				if (!globToCheck.Any())
				{
					AnsiConsole.MarkupInterpolated($"[bold yellow]Warning:[/] No files found for glob \"{glob}\"\n");
				}
				else
				{
					inputs.AddRange(globToCheck.Select(i => Path.Join(glob[..lastSlash], i)));
				}
			}

			foreach (var i in new List<string>([.. inputs, .. GetCoreLib()]))
			{
				if (!ParseFile(i))
				{
					errored = true;
				}
			}

			if (errored)
			{
				return false;
			}

			// Do class decls before
			foreach (var i in Roots)
			{
				if (!i.Value.BuildSymbols())
				{
					errored = true;
				}
			}

			foreach (var i in Roots)
			{
				if (!i.Value.CompileFunctions(out var funcs))
				{
					errored = true;
				}
				else
				{
					IR.AddFunctions(funcs);
				}
			}

			if (GlobalInitFunc.FirstBlock.Instructions.Count != 0)
			{
				GlobalInitFunc.Add(new ReturnInsn());
				GlobalInitFunc.Finish();
				IR.AddFunctions(GlobalInitFunc);
			}

			if (errored)
			{
				return false;
			}

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
				if (error.Errored)
				{
					return false;
				}

				Roots[filename] = (RootNode)visitor.Visit(root);
			}
			catch
			{
				if (!error.Errored)
				{
					throw;
				}
			}

			if (error.Errored)
			{
				return false;
			}

			return true;
		}

		public bool WrapError(LocationRange loc, Action cb)
		{
			try
			{
				cb();
			}
			catch (GeodeError e)
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
			catch (GeodeError e)
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

		protected virtual void RegisterGlobals()
		{
			Register(PrimitiveType.Bool);
			Register(PrimitiveType.Byte);
			Register(PrimitiveType.Short);
			Register(PrimitiveType.Int);
			Register(PrimitiveType.Long);
			Register(PrimitiveType.Float);
			Register(PrimitiveType.Double);
			Register(PrimitiveType.String);
			Register(PrimitiveType.List);
			Register(PrimitiveType.Compound);

			Register(new TargetSelectorType());

			Register(new Print());
			Register(new CountOf());
			Register(new ListAdd());
			Register(new ListSize());
			Register(new StringLength());

			IR.AddSymbol(new("builtin:true", LocationRange.None, new LiteralValue(true)));
			IR.AddSymbol(new("builtin:false", LocationRange.None, new LiteralValue(false)));
			IR.AddSymbol(new("amethyst:stack", LocationRange.None, new StorageValue(IR.RuntimeID, "stack", new ListType(PrimitiveType.Compound))));
		}

		protected FunctionContext GetGlobalInitFunc() => new(this, new(new("amethyst", GeodeBuilder.InternalPath + "/" + GeodeBuilder.RandomString), FunctionType.VoidFunc), ["minecraft:load"], LocationRange.None, hasTagPriority: true);

		public void Register(Intrinsic func) => IR.Symbols[func.ID] = new(func.ID, LocationRange.None, func);
		public void Register(TypeSpecifier type) => IR.Types[type.ID] = new(type.ID, LocationRange.None, type);

		public static IEnumerable<string> GetCoreLib()
        {
			var bundledLocation = GetAllAmethystFilesFromDirectory(Path.Join(AppContext.BaseDirectory, "core"));

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return [.. bundledLocation, .. GetAllAmethystFilesFromDirectory("/usr/share/amethyst/core")];
            }

			return bundledLocation;
		}

		public static IEnumerable<string> GetAllAmethystFilesFromDirectory(string dir) => Glob.Files(dir, "**/*.ame").Select(i => Path.Join(dir, i));
	}
}
