using Amethyst.Antlr;
using Amethyst.AST;
using Amethyst.AST.Intrinsics;
using Amethyst.Cli;
using Amethyst.IR.Types;
using Antlr4.Runtime;
using Datapack.Net.Function;
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

		public string CoreLibPath { get; private set; }

		public GeodeBuilder IR { get; }

		private Dictionary<string, string> Files { get; } = [];

		public Compiler(BuildOptions opts)
		{
			Options = opts;
			IR = new(Options, this, this, "amethyst");

			GlobalInitFunc = GetGlobalInitFunc();
			RegisterGlobals();
		}

		public bool Compile()
		{
			var errored = false;

			LoadFiles();

			foreach (var (k, v) in Files)
			{
				if (!ParseFile(k, v))
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

			if (GlobalInitFunc.Start.Instructions.Count != 0)
			{
				GlobalInitFunc.Add(new ReturnInsn());
				GlobalInitFunc.Finish();
				IR.AddFunctions(GlobalInitFunc);
			}

			if (errored || !IR.Compile())
			{
				return false;
			}

			if (Options.DumpCommands)
			{
				foreach (var i in IR.Functions)
				{
					if (i.Decl.ID.Namespace != "amethyst")
                    {
                        i.FancyPrintCommands();
                    }
				}
			}

			return true;
		}

		public void LoadFiles()
        {
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
				LoadFile(i);
			}
		}

		public void LoadFile(string path) => LoadFile(path, File.ReadAllText(path));
		public void LoadFile(string path, string file) => Files[path] = file;

		public string GetFile(string path)
        {
            if (Files.TryGetValue(path, out var file))
            {
                return file;
            }

			file = File.ReadAllText(path);
			Files[path] = file;
			return file;
        }

		public bool ParseFile(string filename, string file)
		{
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

		[System.Diagnostics.DebuggerNonUserCode] // Hide this on the call stack
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

		[System.Diagnostics.DebuggerNonUserCode] // Hide this on the call stack
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

		public string PathToMap(string path)
        {
			if (path.StartsWith(CoreLibPath))
            {
				// Includes a slash probably
                return $"@std{path[CoreLibPath.Length..]}";
            }

            return path;
        }

		public string MapToPath(string mappedPath)
        {
            if (mappedPath.StartsWith("@std"))
            {
                return Path.Combine(CoreLibPath, mappedPath["@std".Length..]);
            }

			return mappedPath;
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
			Register(EntityType.Dummy);

			Register(new UnsafeStringType());
			Register(new IntRangeType());
			Register(new FloatRangeType());
			Register(new TargetSelectorType());

			Register(new Print());
			Register(new CountOf());
			Register(new ListAdd());
			Register(new ListSize());
			Register(new StringLength());
			Register(new Summon());
			Register(new Kill());
			Register(new AST.Intrinsics.Random());
			Register(new Contains());

			IR.AddSymbol(new("builtin:true", LocationRange.None, new LiteralValue(true)));
			IR.AddSymbol(new("builtin:false", LocationRange.None, new LiteralValue(false)));
			IR.AddSymbol(new("amethyst:stack", LocationRange.None, new StorageValue(IR.RuntimeID, "stack", new ListType(PrimitiveType.Compound))));

			IR.Register(new Score("amethyst_id", "dummy"));
		}

		protected FunctionContext GetGlobalInitFunc() => new(this, new(new("amethyst", GeodeBuilder.InternalPath + "/" + GeodeBuilder.RandomString), FunctionType.VoidFunc, LocationRange.None), ["minecraft:load"], LocationRange.None, hasTagPriority: true);

		public void Register(Intrinsic func) => IR.Symbols[func.ID] = new(func.ID, LocationRange.None, func);
		public void Register(TypeSpecifier type) => IR.Types[type.ID] = new(type.ID, LocationRange.None, type);

		public IEnumerable<string> GetCoreLib()
        {
			if (AttemptCoreLibLoad(Path.Join(AppContext.BaseDirectory, "std"), out var normal))
            {
                return normal;
            }
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && AttemptCoreLibLoad("/usr/share/amethyst/std", out var linux))
            {
                return linux;
            }

			throw new FileNotFoundException("The standard library could not be loaded. Try reinstalling Amethyst");
		}

		private bool AttemptCoreLibLoad(string path, out IEnumerable<string> files)
        {
            files = GetAllAmethystFilesFromDirectory(path);

			if (files.Any())
            {
                CoreLibPath = path;
				return true;
            }

			return false;
		}

		public static IEnumerable<string> GetAllAmethystFilesFromDirectory(string dir) => Glob.Files(dir, "**/*.ame").Select(i => Path.Join(dir, i));
	}
}
