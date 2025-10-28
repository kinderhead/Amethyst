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
		public override int Execute(CommandContext context, DaemonRunOptions settings) => Server.RunDatapack(settings);
	}
}
