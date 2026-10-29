using System.ComponentModel;
using Amethyst.Daemon;
using Amethyst.PM;
using Datapack.Net.Pack;
using Spectre.Console.Cli;

namespace Amethyst.Cli.Shards
{
    public class BuildShardSettings : BaseShardSettings, IAmethystOptions
    {
        [CommandOption("--run")]
        [Description("Run the datapack if built successfully.")]
        public bool Run { get; set; }

        [CommandOption("-d|--debug")]
        [Description("Enable debug checks.")]
        public bool Debug { get; set; }

        [CommandOption("--dump-ir")]
        [Description("Dump Geode IR.")]
        public bool DumpIR { get; set; }

        [CommandOption("-O")]
        [Description("Set the opimization level.")]
        [DefaultValue(1)]
        public int OptimizationLevel { get; set; }

        [CommandOption("-c|--dump-cmd")]
        [Description("Dump non-std functions to console.")]
        public bool DumpCommands { get; set; }

        [CommandOption("--disable-compute")]
        [Description("Disable generating and using number providers and /compute.")]
        public bool DisableCompute { get; set; }

        [CommandOption("-p|--pack-format")]
        [Description("Data pack version to support. 0.0 to use the project default.")]
        [DefaultValue("0.0")]
        public PackFormat PackFormat { get; set; }

        [CommandOption("-I|--include")]
        [Description("Paths to data packs to include references to.")]
        public string[] Include { get; set; } = [];

        [CommandOption("-L|--link")]
        [Description("Paths to data packs to link into the output.")]
        public string[] Link { get; set; } = [];

        // IAmethystOptions settings
        public string Output { get; set; } = "";
        public string? Data { get; set; }
        public string? Description { get; set; }
        public string[] Inputs { get; set; } = [];
    }

    public class BuildShardCommand : Command<BuildShardSettings>
    {
        public override int Execute(CommandContext context, BuildShardSettings settings, CancellationToken cancellationToken)
        {
            if (!ProjectDefinition.Deserialize(settings.ShardFile, out var project)) return 1;

            Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(settings.ShardFile)) ??
                                           throw new FormatException($"Invalid path {settings.ShardFile}");

            settings.Output = Path.Join(Environment.CurrentDirectory, "build", $"{project.Name.Replace('/', '-').Replace(':', '-')}@{project.Version}.zip");
            settings.Inputs = [Path.Join(project.SourceDir, "**/*.ame")];
            settings.Data = project.DataDir;
            settings.Description = project.Description;

            if (settings.PackFormat == new PackFormat("0.0")) settings.PackFormat = project.PackFormat;

            var newLink = new List<string>();
            var newInclude = new List<string>();

            if (Cluster.WithCli(() =>
                {
                    foreach (var i in project.Dependencies.ResolveDependencies(project))
                    {
                        if (i.Type == LinkType.Dynamic) newInclude.Add(i.ZipPath);
                        else newLink.Add(i.ZipPath);
                    }
                }) == 1) return 1;


            settings.Link = [.. settings.Link, .. newLink];
            settings.Include = [.. settings.Include, .. newInclude];

            Directory.CreateDirectory(Path.Join(Environment.CurrentDirectory, "build"));

            var compiler = new Compiler(settings);

            if (!compiler.CompileWithProgress()) return 1;
            if (settings.Run) Runner.RunDatapack(new() { Datapacks = [settings.Output, .. settings.Include] }, compiler);

            return 0;
        }
    }
}