using Amethyst.Daemon;
using Datapack.Net.Pack;
using Spectre.Console;
using Spectre.Console.Cli;
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

		// IAmethystOptions settings
		public string Output { get; set; }
		public PackFormat PackVersion { get; set; }
		public string[] Inputs { get; set; }
	}

	public class BuildProjectCommand : Command<BuildProjectSettings>
	{
		public override int Execute(CommandContext context, BuildProjectSettings settings,
			CancellationToken cancellationToken)
		{
			var project = ProjectDefinition.Deserialize(settings.ShardFile);
			Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(settings.ShardFile)) ??
			                               throw new FormatException($"Invalid path {settings.ShardFile}");

			settings.Output = Path.Join(Environment.CurrentDirectory, "build", $"{project.Name}-{project.Version}.zip");
			settings.PackVersion = project.PackFormat;
			settings.Inputs = [Path.Join(project.SourceDir, "**/*.ame")];

			Directory.CreateDirectory(Path.Join(Environment.CurrentDirectory, "build"));

			var compiler = new Compiler(settings);
			var success = false;

			AnsiConsole.Status().Start("[darkviolet]Compiling...[/]", _ =>
			{
				success = compiler.Compile();
			});

			if (!success)
			{
				return 1;
			}

			if (settings.Run)
			{
				Runner.RunDatapack(new() { Datapack = settings.Output }, compiler);
			}

			return 0;
		}
	}
}