using System.ComponentModel;
using Spectre.Console.Cli;

namespace Amethyst.Cli.Data
{
    public class BaseDataSettings : CommandSettings
    {
        [CommandArgument(0, "[output]")]
        [Description("File to output to. Defaults to the standard output.")]
        public string? Output { get; set; }
    }
}