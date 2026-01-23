using System.Reflection;
using Amethyst.Cli;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;
using Tmds.Utils;

namespace Amethyst
{
	internal class Program
	{
		private static int Main(string[] args)
		{
			if (ExecFunction.IsExecFunctionCommand(args))
			{
				return ExecFunction.Program.Main(args);
			}

#if DEBUG
			Console.Clear(); // Thanks Visual Studio for being sad
#endif

			AnsiConsole.MarkupLine("[yellow]Amethyst is currently in development. Report issues at [aqua underline]https://github.com/kinderhead/Amethyst/issues[/].[/]\n");

#pragma warning disable IL3050
			var app = new CommandApp();
#pragma warning restore IL3050

			app.SetDefaultCommand<DefaultCommand>();
			app.Configure(config =>
            {
				config.SetApplicationName("amethyst");
				config.Settings.MaximumIndirectExamples = 100;
				config.Settings.HelpProviderStyles?.Description?.Header = "default";
				config.Settings.HelpProviderStyles?.Usage?.Header = "default";
				config.Settings.HelpProviderStyles?.Examples?.Header = "default";
				config.Settings.HelpProviderStyles?.Arguments?.Header = "default";
				config.Settings.HelpProviderStyles?.Arguments?.RequiredArgument = "aqua";
				config.Settings.HelpProviderStyles?.Arguments?.OptionalArgument = "silver";
				config.Settings.HelpProviderStyles?.Options?.Header = "default";
				config.Settings.HelpProviderStyles?.Commands?.Header = "default";

				config.SetExceptionHandler((ex, resolver) =>
                {
					AnsiConsole.MarkupLineInterpolated($"[red]{ex.GetType().Name}: {ex.Message}[/]");
                    return 1;
                });

				config.AddCommand<BuildCommand>("build")
					.WithDescription("Amethyst compiler.")
					.WithExample("build", "examples/test.ame", "-o", "datapack.zip")
					.WithExample("build", "tests/*.ame", "-o", "tests.zip");

				config.AddCommand<DaemonSetupCommand>("setup")
					.WithDescription("Amethyst runtime Minecraft server setup.")
					.WithExample("setup", "--eula");

				config.AddCommand<DaemonRunCommand>("run")
					.WithDescription("Run a datapack.")
					.WithExample("run test.zip");

				config.AddCommand<DaemonLaunchCommand>("daemon")
					.WithDescription("Run the Amethyst runtime Minecraft server without a timeout.")
					.WithExample("daemon")
					.WithExample("daemon -c \"op steve\"");
			});

			return app.Run(args);
		}
	}
}
