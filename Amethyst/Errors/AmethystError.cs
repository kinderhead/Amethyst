using Amethyst.AST;
using Spectre.Console;

namespace Amethyst.Errors
{
	public class AmethystError(string msg) : Exception
	{
		public readonly string RawMessage = msg;

		public override string Message => $"{GetType().Name}: {RawMessage}";

		public virtual void Display(IFileHandler handler, LocationRange loc, bool last = true)
		{
			var lines = handler.Files[loc.Start.File].Split('\n');

			var lastCol = loc.End.Column;
			if (loc.Start.Line != loc.End.Line) lastCol = lines[loc.Start.Line - 1].Length;

			AnsiConsole.MarkupLine($"[red]Error at {loc.Start}\n│[/]");
			AnsiConsole.MarkupLine("[red]│[/]   [turquoise2]" + lines[loc.Start.Line - 1].EscapeMarkup() + "[/]");
			AnsiConsole.MarkupLine("[red]│[/]   " + new string(' ', loc.Start.Column - 1) + $"[red]{new string('~', lastCol - loc.Start.Column + 1)}[/]");
			AnsiConsole.MarkupLine($"[red]{(last ? "└─" : "│ ")}  {GetType().Name}:[/] [yellow]{RawMessage.EscapeMarkup()}[/]{(last ? "\n\n" : "")}");
		}
	}

	public class DoubleAmethystError(string msg1, LocationRange loc2, string msg2) : AmethystError(msg1)
	{
		public readonly LocationRange Location2 = loc2;
		public readonly string RawMessage2 = msg2;

		public override void Display(IFileHandler handler, LocationRange loc, bool last = true)
		{
			base.Display(handler, loc, false);

			var lines = handler.Files[Location2.Start.File].Split('\n');

			var lastCol = Location2.End.Column;
			if (Location2.Start.Line != Location2.End.Line) lastCol = lines[Location2.Start.Line - 1].Length;

			AnsiConsole.MarkupLine($"[red]│\n│[/]   [green]{RawMessage2} {Location2.Start}[/]\n[red]│[/]");
			AnsiConsole.MarkupLine("[red]│[/]   [turquoise2]" + lines[Location2.Start.Line - 1] + "[/]");
			AnsiConsole.MarkupLine("[red]└─[/]  " + new string(' ', Location2.Start.Column - 1) + $"[violet]{new string('~', lastCol - Location2.Start.Column + 1)}[/]{(last ? "\n\n" : "")}");
		}
	}

	public class EmptyAmethystError() : AmethystError("")
	{
		public override void Display(IFileHandler handler, LocationRange loc, bool lastNewlines = true)
		{

		}
	}
}
