using Datapack.Net.Pack;
using Geode;
using Spectre.Console.Cli;
using System;
using System.ComponentModel;

namespace Amethyst.Cli
{
    public class BaseProjectSettings : CommandSettings
    {
        [CommandOption("-f|--shard-file")]
        [Description("Project file to use.")]
        [DefaultValue(Compiler.SHARD_PROJECT)]
        public string ShardFile { get; set; }
    }

    public class BuildProjectSettings : BaseProjectSettings, IOptions
    {
        [CommandOption("-d|--debug")]
        [Description("Enable debug checks.")]
        public bool Debug { get; set; }

        [CommandOption("--dump-ir")]
        [Description("Dump Geode IR and don't compile to datapack.")]
        public bool DumpIR { get; set; }

        [CommandOption("-O")]
        [Description("Set the opimization level.")]
        [DefaultValue(1)]
        public int OptimizationLevel { get; set; }

        [CommandOption("-c|--dump-cmd")]
        [Description("Dump non-std functions to console.")]
        public bool DumpCommands { get; set; }

        [CommandOption("--run")]
        [Description("Run the datapack if built successfully.")]
        public bool Run { get; set; }

        // IOptions settings
		public string Output { get; set; }
		public PackVersion PackVersion { get; set; }
	}

	public class BuildProjectCommand : Command<BuildProjectSettings>
	{
		public override int Execute(CommandContext context, BuildProjectSettings settings, CancellationToken cancellationToken)
        {
            var project = ProjectDefinition.Deserialize(settings.ShardFile);
            Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(settings.ShardFile)) ?? throw new FormatException($"Invalid path {settings.ShardFile}");

            return 0;
        }
	}
}
