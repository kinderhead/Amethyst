using System.ComponentModel;
using Amethyst.PM;
using Datapack.Net.Pack;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Amethyst.Cli.Clusters
{
    public class ClusterAddSettings : BaseClusterSettings
    {
        [CommandOption("-s|--shard-file")]
        [Description("Project file to autofill missing arguments.")]
        [DefaultValue(Compiler.SHARD_PROJECT)]
        public string ShardFile { get; set; } = "";

        [CommandOption("-v|--version")]
        [Description("Version to publish.")]
        public string? Version { get; set; }

        [CommandOption("-p|--pack-format")]
        [Description("Pack format.")]
        public PackFormat? Format { get; set; }

        [CommandOption("-d|--dependency")]
        [Description("Required dynamic dependencies in the format namespace:path/name@version.")]
        public string[] Dependencies { get; set; } = [];

        [CommandArgument(0, "<link>")]
        [Description("Link to compiled data pack.")]
        public required string Link { get; set; }
    }

    public class ClusterAddCommand : Command<ClusterAddSettings>
    {
        public override int Execute(CommandContext context, ClusterAddSettings settings, CancellationToken cancellationToken)
        {
            ProjectDefinition? project = null;
            if (ProjectDefinition.Deserialize(settings.ShardFile, out var ret))
            {
                // Keep as nullable
                project = ret;
            }

            settings.Format ??= project?.PackFormat;
            settings.Version ??= project?.Version.ToString();

            if (project is not null) settings.Dependencies = [.. settings.Dependencies, .. project.Dependencies.GetDynamicClusterDependencies().Select(i => $"{i.Key}@{i.Value}")];

            if (settings.Format is null)
            {
                AnsiConsole.MarkupLine("[red]Error:[/] [yellow]Missing shard format[/]");
                return 1;
            }

            if (settings.Version is null)
            {
                AnsiConsole.MarkupLine("[red]Error:[/] [yellow]Missing shard version[/]");
                return 1;
            }

            return Cluster.WithCli(() =>
            {
                var cluster = Cluster.From(settings.ClusterFile);
                cluster.Add(settings.Version, settings.Format.Value, settings.Link,
                    new(settings.Dependencies.Select(i => new KeyValuePair<string, string>(i.Split('@')[0], i.Split('@')[1]))));
                cluster.Save();

                AnsiConsole.MarkupLineInterpolated($"Added {settings.Version} to {settings.ClusterFile}");
            });
        }
    }
}