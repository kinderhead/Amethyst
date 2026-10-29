using System.ComponentModel;
using Amethyst.PM;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Amethyst.Cli.Shards
{
    public class ShardAddSettings : BaseShardSettings
    {
        [CommandOption("-t|--type")]
        [Description("Link type. Either static or dynamic.")]
        [DefaultValue(LinkType.Static)]
        public LinkType Type { get; set; }

        [CommandArgument(0, "<shard>")]
        [Description("Shard to add as a dependency.")]
        public required string Add { get; set; }
    }

    public class ShardAddCommand : Command<ShardAddSettings>
    {
        public override int Execute(CommandContext context, ShardAddSettings settings, CancellationToken cancellationToken)
        {
            if (!ProjectDefinition.Deserialize(settings.ShardFile, out var project)) return 1;

            return Cluster.WithCli(() =>
            {
                var cluster = Cluster.From(Cluster.ROOT_CLUSTER);
                var entry = settings.Add.Contains('@') ? cluster.Query(settings.Add) : cluster.QueryLatest(settings.Add, project.PackFormat);
                project.Dependencies.Set(new PackageEntry(entry.ID, settings.Type, entry.Version));
                File.WriteAllText(settings.ShardFile, project.Serialize());

                AnsiConsole.MarkupLineInterpolated($"[green3_1]Added [gold1]{entry.ID}@{entry.Version}[/] as a dependency.[/]");
            });
        }
    }
}