using Spectre.Console;

namespace Amethyst
{
	internal class Program
	{
		private static void Main(string[] args)
		{
#if DEBUG
			Console.Clear(); // Thanks Visual Studio for being sad
#endif
			AnsiConsole.MarkupLine("[yellow]Amethyst is currently in development. Report issues at [aqua underline]https://github.com/kinderhead/Amethyst/issues[/].[/]\n");

			if (!new Compiler(args).Compile())
			{
				Environment.Exit(1);
			}
		}
	}
}
