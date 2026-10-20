using System.ComponentModel;
using Amethyst.Daemon;
using Datapack.Net.Pack;
using Spectre.Console.Cli;

namespace Amethyst.Cli
{
    public class BaseProjectSettings : CommandSettings
    {
        [CommandOption("-f|--shard-file")]
        [Description("Project file to use.")]
        [DefaultValue(Compiler.SHARD_PROJECT)]
        public string ShardFile { get; set; } = "";
    }

    public class BuildProjectSettings : BaseProjectSettings, IAmethystOptions
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

        [CommandOption("-p|--pack-version")]
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

    public class BuildProjectCommand : Command<BuildProjectSettings>
    {
        public override int Execute(CommandContext context, BuildProjectSettings settings, CancellationToken cancellationToken)
        {
            var project = ProjectDefinition.Deserialize(settings.ShardFile);
            Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(settings.ShardFile)) ??
                                           throw new FormatException($"Invalid path {settings.ShardFile}");

            settings.Output = Path.Join(Environment.CurrentDirectory, "build", $"{project.Name}-{project.Version}.zip");
            settings.Inputs = [Path.Join(project.SourceDir, "**/*.ame")];
            settings.Data = project.DataDir;
            settings.Description = project.Description;

            if (settings.PackFormat == new PackFormat("0.0")) settings.PackFormat = project.PackFormat;

            Directory.CreateDirectory(Path.Join(Environment.CurrentDirectory, "build"));

            var compiler = new Compiler(settings);

            if (!compiler.CompileWithProgress()) return 1;
            if (settings.Run) Runner.RunDatapack(new() { Datapacks = [settings.Output, .. settings.Include] }, compiler);

            return 0;
        }
    }
}