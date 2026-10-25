using System.ComponentModel;
using Spectre.Console.Cli;

namespace Amethyst.Cli.Shards
{
    public class BaseShardSettings : CommandSettings
    {
        [CommandOption("-f|--shard-file")]
        [Description("Project file to use.")]
        [DefaultValue(Compiler.SHARD_PROJECT)]
        public string ShardFile { get; set; } = "";
    }
}