using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Amethyst.Daemon;
using Spectre.Console.Cli;

namespace Amethyst.Cli
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
    public class DaemonRunOptions : CommandSettings
    {
        [CommandArgument(0, "<data pack>")]
        [Description("Path to a data pack to run.")]
        public required string Datapack { get; set; }
    }

	public class DaemonRunCommand : Command<DaemonRunOptions>
	{
		public override int Execute(CommandContext context, DaemonRunOptions settings, CancellationToken cancellationToken)
        {
            var compiler = new Compiler(new BuildOptions() { Inputs = [], Output = "" });
            compiler.GetCoreLib(); // Find an std path
            return Runner.RunDatapack(settings, compiler);
        }
	}
}
