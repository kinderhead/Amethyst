using Amethyst.AST;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Amethyst.Errors
{
	public class AmethystError(LocationRange loc, string msg) : Exception
	{
		public readonly LocationRange Location = loc;
		public readonly string RawMessage = msg;

		public override string Message => $"{GetType().Name} at {Location}: {RawMessage}";

		public virtual void Display(IFileHandler handler, bool last = true)
		{
			var lines = handler.Files[Location.Start.File].Split('\n');

			var lastCol = Location.End.Column;
			if (Location.Start.Line != Location.End.Line) lastCol = lines[Location.Start.Line - 1].Length;

			AnsiConsole.MarkupLine($"[red]Error at {Location.Start}\n│[/]");
			AnsiConsole.MarkupLine("[red]│[/]   [turquoise2]" + lines[Location.Start.Line - 1] + "[/]");
			AnsiConsole.MarkupLine("[red]│[/]   " + new string(' ', Location.Start.Column - 1) + $"[red]{new string('~', lastCol - Location.Start.Column + 1)}[/]");
			AnsiConsole.MarkupLine($"[red]{(last ? "└─" : "│ ")}  {GetType().Name}:[/] [yellow]{RawMessage}[/]{(last ? "\n\n" : "")}");
		}
	}

	public class DoubleAmethystError(LocationRange loc1, string msg1, LocationRange loc2, string msg2) : AmethystError(loc1, msg1)
	{
		public readonly LocationRange Location2 = loc2;
		public readonly string RawMessage2 = msg2;

		public override void Display(IFileHandler handler, bool last = true)
		{
			base.Display(handler, false);

			var lines = handler.Files[Location2.Start.File].Split('\n');

			var lastCol = Location2.End.Column;
			if (Location2.Start.Line != Location2.End.Line) lastCol = lines[Location2.Start.Line - 1].Length;

			AnsiConsole.MarkupLine($"[red]│\n│[/]   [green]{RawMessage2} {Location2.Start}[/]\n[red]│[/]");
			AnsiConsole.MarkupLine("[red]│[/]   [turquoise2]" + lines[Location2.Start.Line - 1] + "[/]");
			AnsiConsole.MarkupLine("[red]└─[/]  " + new string(' ', Location2.Start.Column - 1) + $"[violet]{new string('~', lastCol - Location2.Start.Column + 1)}[/]{(last ? "\n\n" : "")}");
		}
	}

	public class EmptyAmethystError() : AmethystError(new(new("", 0, 0), new("", 0, 0)), "")
	{
		public override void Display(IFileHandler handler, bool lastNewlines = true)
		{
			
		}
	}
}
