using Amethyst.PM;
using Spectre.Console.Cli;

namespace Amethyst.Cli.Clusters
{
    public class ClusterInitCommand : Command<BaseClusterSettings>
    {
        public override int Execute(CommandContext context, BaseClusterSettings settings, CancellationToken cancellationToken)
        {
            return Cluster.WithCli(() =>
            {
                var path = Path.GetDirectoryName(settings.ClusterFile);
                if (!string.IsNullOrEmpty(path)) Directory.CreateDirectory(path);
                Cluster.New(settings.ClusterFile).Save();
            });
        }
    }
}