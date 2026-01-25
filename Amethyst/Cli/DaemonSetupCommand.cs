using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Amethyst.Daemon;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Amethyst.Cli
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
    public class DaemonSetupOptions : CommandSettings
    {
        [CommandOption("--eula")]
        [Description("Accept the EULA.")]
        public bool EULA { get; set; }

        [CommandOption("-j|--java")]
        [Description("JVM to use.")]
        [DefaultValue("java")]
        public string Java { get; set; }

        [CommandOption("-m|--memory")]
        [Description("Max amount of memory for the server via -Xmx.")]
        [DefaultValue("2G")]
        public string Memory { get; set; }

        [CommandOption("-v|--version")]
        [Description("Minecraft server version.")]
        [DefaultValue("1.21.11")]
        public string MinecraftVersion { get; set; }

        [CommandOption("-p|--port")]
        [Description("Minecraft server port. Port + 1 will be used for rcon.")]
        [DefaultValue(25600)]
        public int Port { get; set; }

        [CommandOption("-t|--timeout")]
        [Description("Minutes the server stays alive with no activity.")]
        [DefaultValue(30.0f)]
        public float Timeout { get; set; }
    }

	public class DaemonSetupCommand : Command<DaemonSetupOptions>
	{
		public override int Execute(CommandContext context, DaemonSetupOptions settings, CancellationToken cancellationToken) => Server.Setup(settings).Result;
	}
}
