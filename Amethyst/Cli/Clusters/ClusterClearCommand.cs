using Amethyst.PM;
using Spectre.Console.Cli;

namespace Amethyst.Cli.Clusters
{
    public class ClusterClearCommand : Command
    {
        public override int Execute(CommandContext context, CancellationToken cancellationToken)
        {
            Directory.Delete(Cluster.LocalClusterFolder, true);
            return 0;
        }
    }
}