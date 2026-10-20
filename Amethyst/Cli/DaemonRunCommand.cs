using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Amethyst.Daemon;
using Spectre.Console.Cli;

namespace Amethyst.Cli
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
    public class DaemonRunOptions : CommandSettings
    {
        [CommandArgument(0, "<data packs>")]
        [Description("Path to data packs to run.")]
        public required string[] Datapacks { get; set; }
    }

    public class DaemonRunCommand : Command<DaemonRunOptions>
    {
        public override int Execute(CommandContext context, DaemonRunOptions settings, CancellationToken cancellationToken)
        {
            var compiler = new Compiler(new CompileOptions { Inputs = [], Output = "" });
            compiler.GetCoreLib(); // Find the std path
            return Runner.RunDatapack(settings, compiler);
        }
    }
}