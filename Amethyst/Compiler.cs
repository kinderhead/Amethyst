using System.Diagnostics;
using System.Runtime.InteropServices;
using Amethyst.Antlr;
using Amethyst.AST;
using Amethyst.AST.Intrinsics;
using Amethyst.GC;
using Amethyst.IR;
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
using Random = Amethyst.AST.Intrinsics.Random;

namespace Amethyst
{
    public class Compiler : ICompiler, IFileHandler
    {
        public const string SHARD_PROJECT = "shard.json";

        public readonly FunctionContext GlobalInitFunc;
        public readonly IAmethystOptions Options;
        public readonly Dictionary<string, RootNode> Roots = [];

        public readonly TypeHandler TypeHandler;

        private string? coreLibPath;

        public Compiler(IAmethystOptions opts)
        {
            Options = opts;
            TypeHandler = new(this);
            IR = new(Options, this, this, "amethyst");

            GlobalInitFunc = GetGlobalInitFunc();
            RegisterGlobals();
        }

        public string CoreLibPath => coreLibPath ?? throw new("No core library has been loaded");

        private Dictionary<string, string> Files { get; } = [];
        public GeodeBuilder IR { get; }

        [DebuggerNonUserCode] // Hide this on the call stack
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

        [DebuggerNonUserCode] // Hide this on the call stack
        public bool WrapError<T>(LocationRange loc, Func<T> cb, out T ret)
        {
            T realRet = default!;
            var success = WrapError(loc, () => realRet = cb());
            ret = realRet;

            return success;
        }

        [DebuggerNonUserCode] // Hide this on the call stack
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

        public string GetFile(string path)
        {
            if (Files.TryGetValue(path, out var file)) return file;

            file = File.ReadAllText(path);
            Files[path] = file;
            return file;
        }

        public string PathToMap(string path) =>
            // Includes a slash probably
            path.StartsWith(CoreLibPath) ? $"@std{path[CoreLibPath.Length..]}" : path;

        public string MapToPath(string mappedPath) => mappedPath.StartsWith("@std") ? Path.Combine(CoreLibPath, mappedPath["@std".Length..]) : mappedPath;

        public bool Compile(StatusContext? ctx = null)
        {
            var errored = false;

            LoadFiles();

            foreach (var (k, v) in Files)
            {
                ctx?.Status = $"[darkviolet]Parsing {k}...[/]";
                if (!ParseFile(k, v)) errored = true;
            }

            if (errored) return false;

            foreach (var i in Roots)
            {
                ctx?.Status = $"[darkviolet]Processing {i.Key.EscapeMarkup()}...[/]";
                if (!i.Value.BuildSoftTypes()) errored = true;
            }

            if (errored) return false;

            ctx?.Status = "[darkviolet]Building symbols...[/]";
            if (!TypeHandler.GenerateSymbols()) errored = true;

            TypeHandler.GenerateTypeInfo(GlobalInitFunc);
            GenerateGCMarkFunction();

            foreach (var i in Roots)
            {
                ctx?.Status = $"[darkviolet]Compiling {i.Key.EscapeMarkup()}...[/]";
                if (!i.Value.CompileFunctions(out var funcs)) errored = true;
                else IR.AddFunctions(funcs);
            }

            if (GlobalInitFunc.Start.Instructions.Count != 0)
            {
                GlobalInitFunc.Add(new ReturnInsn());
                GlobalInitFunc.Finish();
                IR.AddFunctions(GlobalInitFunc);
            }

            ctx?.Status = "[darkviolet]Generating commands...[/]";

            if (errored || !IR.Compile()) return false;

            if (Options.DumpCommands)
            {
                foreach (var i in IR.Functions)
                {
                    if (i.Decl.ID.Namespace != "amethyst"
#if DEBUG
                        || i.Tags.Any(i => i.ToString().Contains("debug"))
#endif
                       ) i.FancyPrintCommands();
                }
            }

            ctx?.Status = "[darkviolet]Done![/]";

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
                    if (glob[i] == '/') lastSlash = i;

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

                string[] globToCheck = lastSlash == 0
                    ? [.. Glob.Files(Directory.GetCurrentDirectory(), glob)]
                    : [.. Glob.Files(glob[..lastSlash], glob[(lastSlash + 1)..])];

                if (globToCheck.Length == 0) AnsiConsole.MarkupInterpolated($"[bold yellow]Warning:[/] No files found for glob \"{glob}\"\n");
                else inputs.AddRange(globToCheck.Select(i => Path.Join(glob[..lastSlash], i)));
            }

            foreach (var i in new List<string>([.. inputs, .. GetCoreLib()]))
            {
                LoadFile(i);
            }
        }

        public void LoadFile(string path) => LoadFile(path, File.ReadAllText(path));
        public void LoadFile(string path, string file) => Files[path] = file;

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
                if (error.Errored) return false;
                Roots[filename] = (RootNode)visitor.Visit(root);
            }
            catch
            {
                if (!error.Errored) throw;
            }

            return !error.Errored;
        }

        private void GenerateGCMarkFunction()
        {
            var ctx = new FunctionContext(this,
                new(new("amethyst", GeodeBuilder.INTERNAL_PATH + "/" + GeodeBuilder.UniqueString), FunctionType.VoidFunc,
                    LocationRange.None), ["amethyst:gc/mark"], LocationRange.None, true);

            foreach (var (_, val) in IR.Symbols)
            {
                GCHelper.Mark(val.Value, ctx);
            }

            if (ctx.Start.Instructions.Count == 0) return;

            ctx.Add(new ReturnInsn());
            ctx.Finish();
            IR.AddFunctions(ctx);
        }

        private void RegisterGlobals()
        {
            TypeHandler.Register(PrimitiveType.Bool);
            TypeHandler.Register(PrimitiveType.Byte);
            TypeHandler.Register(PrimitiveType.Short);
            TypeHandler.Register(PrimitiveType.Int);
            TypeHandler.Register(PrimitiveType.Long);
            TypeHandler.Register(PrimitiveType.Float);
            TypeHandler.Register(PrimitiveType.Double);
            TypeHandler.Register(PrimitiveType.String);
            TypeHandler.Register(PrimitiveType.List);
            TypeHandler.Register(PrimitiveType.Compound);
            TypeHandler.Register(EntityType.Dummy);

            TypeHandler.Register(new VoidType());
            TypeHandler.Register(new UnsafeStringType());
            TypeHandler.Register(new QStringType());
            TypeHandler.Register(new IntRangeType());
            TypeHandler.Register(new FloatRangeType());
            TypeHandler.Register(new TargetSelectorType());

            Register(new Print());
            Register(new CountOf());
            Register(new ListAdd());
            Register(new ListPop());
            Register(new ListSize());
            Register(new ListWhere());
            Register(new ListAll());
            Register(new ListRemoveAt());
            Register(new StringLength());
            Register(new Summon());
            Register(new Kill());
            Register(new Random());
            Register(new Contains());

            IR.AddSymbol(new("builtin:true", LocationRange.None, new LiteralValue(true)));
            IR.AddSymbol(new("builtin:false", LocationRange.None, new LiteralValue(false)));
            IR.AddSymbol(new("builtin:null", LocationRange.None, new NullValue()));
            IR.AddSymbol(new("amethyst:stack", LocationRange.None, new StorageValue(IR.RuntimeID, "stack", new ListType(PrimitiveType.Compound))));
            IR.AddSymbol(new("amethyst:type_info", LocationRange.None, new StorageValue(IR.RuntimeID, "type_info", new SimpleMapType(PrimitiveType.Compound))));

            IR.Register(new Score("amethyst_id", "dummy"));
        }

        protected FunctionContext GetGlobalInitFunc() => new(this,
            new(new("amethyst", GeodeBuilder.INTERNAL_PATH + "/" + GeodeBuilder.UniqueString), FunctionType.VoidFunc,
                LocationRange.None), ["minecraft:load"], LocationRange.None, true);

        public void Register(Intrinsic func) => IR.Symbols[func.ID] = new(func.ID, LocationRange.None, func);

        public IEnumerable<string> GetCoreLib()
        {
            if (AttemptCoreLibLoad(Path.Join(AppContext.BaseDirectory, "std"), out var normal)) return normal;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && AttemptCoreLibLoad("/usr/share/amethyst/std", out var linux)) return linux;

            throw new FileNotFoundException("The standard library could not be loaded. Try reinstalling Amethyst");
        }

        private bool AttemptCoreLibLoad(string path, out IEnumerable<string> files)
        {
            files = GetAllAmethystFilesFromDirectory(path);

            if (files.Any())
            {
                coreLibPath = path;
                return true;
            }

            return false;
        }

        public static IEnumerable<string> GetAllAmethystFilesFromDirectory(string dir) => Glob.Files(dir, "**/*.ame").Select(i => Path.Join(dir, i));
    }
}