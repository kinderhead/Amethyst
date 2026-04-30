using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Amethyst.Cli
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
    public class ProjectInitCommandOptions : CommandSettings
    {
        [CommandOption("-n|--name")]
        [Description("Name of the project.")]
        public FlagValue<string> Name { get; set; }

        [CommandOption("-d|--description")]
        [Description("Project description.")]
        [DefaultValue("A project made with Amethyst")]
        public string Description { get; set; }

        [CommandOption("-o|--output")]
        [Description("Folder to generate the project.")]
        [DefaultValue(".")]
        public string Output { get; set; }
    }

	public class ProjectInitCommand : Command<ProjectInitCommandOptions>
	{
		public override int Execute(CommandContext context, ProjectInitCommandOptions settings, CancellationToken cancellationToken)
        {
            var project = new ProjectDefinition
            {
                Name = settings.Name.IsSet ? settings.Name.Value : Path.GetFileName(Path.GetFullPath(settings.Output)),
                Description = settings.Description
            };

            var path = Path.Join(settings.Output, Compiler.SHARD_PROJECT);

            if (Path.Exists(path))
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Destination already contains a {Compiler.SHARD_PROJECT}.[/]");
                return 1;
            }

            Directory.CreateDirectory(settings.Output);
            File.WriteAllText(path, project.Serialize());

            AnsiConsole.MarkupLine("Successfully created project.");

            return 0;
        }
	}
}
