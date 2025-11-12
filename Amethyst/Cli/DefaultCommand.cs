using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Amethyst.Cli
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
    public class DefaultCommandOptions : CommandSettings
    {
        [CommandOption("-v|--version")]
        [Description("Print the version and close.")]
        public bool Version { get; set; }
    }

	public class DefaultCommand : Command<DefaultCommandOptions>
	{
		public override int Execute(CommandContext context, DefaultCommandOptions settings, CancellationToken cancellationToken)
        {
            if (settings.Version)
            {
                var version = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version();
                Console.WriteLine($"Amethyst version {version.Major}.{version.Minor}.{version.Build}");
				return 0;
			}
			else
			{
				Console.WriteLine("No arguments specified. Run amethyst --help for more information");
				return 1;
			}
        }
	}
}
