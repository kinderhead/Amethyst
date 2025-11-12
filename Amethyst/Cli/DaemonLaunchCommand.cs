using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Amethyst.Daemon;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Amethyst.Cli
{
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
	public class DaemonLaunchOptions : CommandSettings
    {
		[CommandOption("-c|--command")]
		[Description("Command to send to an existing instance of the Minecraft server.")]
		public string? Command { get; set; }

		[CommandOption("-t|--timeout")]
		[Description("Allow server to automatically stop after some time as configured with \"amethyst setup\"")]
		public bool Timeout { get; set; }
	}

	public class DaemonLaunchCommand : Command<DaemonLaunchOptions>
	{
		public override int Execute(CommandContext context, DaemonLaunchOptions settings, CancellationToken cancellationToken)
        {
            if (settings.Command is null)
            {
				return Server.StartServer(watchOutput: true, timeout: settings.Timeout) ? 0 : 1;
			}
			else
            {
                try
                {
                    var rcon = new Rcon("localhost", Rcon.GetPort());
					rcon.Login(Rcon.Password);
					return rcon.SendCommand(settings.Command) ? 0 : 1;
                }
				catch (Exception e)
                {
                    AnsiConsole.MarkupLineInterpolated($"[red]Error running command: {e.Message}[/]");
					return 1;
                }
            }
        } 
	}
}
