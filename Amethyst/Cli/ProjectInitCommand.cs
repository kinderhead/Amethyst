using Datapack.Net.Pack;
using Datapack.Net.Utils;
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
        public FlagValue<string> Description { get; set; }

        [CommandOption("-p|--pack-version")]
        [Description("Data pack version to support. Setting this flag disables picking a Minecraft definitions package.")]
        public FlagValue<PackVersion> PackVersion { get; set; }

        [CommandOption("-o|--output")]
        [Description("Folder to generate the project.")]
        public string? Output { get; set; }
    }

	public class ProjectInitCommand : Command<ProjectInitCommandOptions>
	{
		public override int Execute(CommandContext context, ProjectInitCommandOptions settings, CancellationToken cancellationToken)
        {
            var output = settings.Output ?? Environment.CurrentDirectory;
            var path = Path.Join(output, Compiler.SHARD_PROJECT);

            if (Path.Exists(path))
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Destination already contains a {Compiler.SHARD_PROJECT}[/]");
                return 1;
            }

            var project = new ProjectDefinition
            {
                Name = settings.Name.IsSet ? settings.Name.Value : AnsiConsole.Ask("[aqua]Project name[/]:", Path.GetFileName(Path.GetFullPath(output)).ToLower()),
                Description = settings.Description.IsSet ? settings.Description.Value : AnsiConsole.Ask("[aqua]Data pack description[/]:", "A project made with Amethyst")
            };

            if (settings.PackVersion.IsSet)
            {
                project.PackVersion = settings.PackVersion.Value;
            }
            else
            {
                var version = AnsiConsole.Prompt(new SelectionPrompt<MinecraftVersion>()
                    .Title("[aqua]Pick a Minecraft version[/]:")
                    .WrapAround()
                    .EnableSearch()
                    .PageSize(6)
                    .UseConverter(version => $"{version} ({SupportedVersions.Versions[version]})")
                    .AddChoices(SupportedVersions.Versions.Keys));
                
                AnsiConsole.MarkupLineInterpolated($"[aqua]Pick a Minecraft version[/]: {version}");
                
                project.PackVersion = SupportedVersions.Versions[version];
                project.Dependencies["minecraft"] = SemVer.Create(version.Major, version.Minor, version.Patch);
            }

            Directory.CreateDirectory(output);
            Directory.CreateDirectory(Path.Join(output, "src"));
            Directory.CreateDirectory(Path.Join(output, "data"));
            File.WriteAllText(path, project.Serialize());

            var mainPath = Path.Join(output, "src", "main.ame");
            if (!Path.Exists(mainPath))
            {
                File.WriteAllText(mainPath, @"#load
void main() {
    print(""Hello world!"");
    amethyst:exit();
}
");
            }

            AnsiConsole.MarkupLine("\n[green]Successfully created project![/]\nRun [darkviolet]amethyst build --run[/] to get started.");
            return 0;
        }
	}
}
