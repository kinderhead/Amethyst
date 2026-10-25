using System.ComponentModel;
using Spectre.Console.Cli;

namespace Amethyst.Cli.Clusters
{
    public class BaseClusterSettings : CommandSettings
    {
        [CommandOption("-f|--cluster-file")]
        [Description("Root cluster file to operate on.")]
        [DefaultValue("cluster.json")]
        public string ClusterFile { get; set; } = "";
    }
}