using Amethyst.AST;
using Spectre.Console;

namespace Amethyst.Errors
{
	public class AmethystError(string msg) : Exception
	{
		public readonly string RawMessage = msg;

		public override string Message => $"{GetType().Name}: {RawMessage}";

		public virtual CompilerMessager Display(IFileHandler handler, LocationRange loc, bool last = true)
		{
			// Console.WriteLine(StackTrace);

			var msg = new CompilerMessager(Color.Red);
			msg.Header($"Error at {loc.Start}");
			msg.AddCode(handler, loc);

			if (last) msg.Final();
			msg.AddContent($"{GetType().Name}: [yellow]{RawMessage.EscapeMarkup()}[/]");

			return msg;

			//AnsiConsole.MarkupLine($"[red]Error at {loc.Start}\n│[/]");
			//AnsiConsole.MarkupLine("[red]│[/]   [turquoise2]" + lines[loc.Start.Line - 1].EscapeMarkup() + "[/]");
			//AnsiConsole.MarkupLine("[red]│[/]   " + new string(' ', loc.Start.Column - 1) + $"[red]{new string('~', lastCol - loc.Start.Column + 1)}[/]");
			//AnsiConsole.MarkupLine($"[red]{(last ? "└─" : "│ ")}  {GetType().Name}:[/] [yellow]{RawMessage.EscapeMarkup()}[/]{(last ? "\n\n" : "")}");
		}

		public override string ToString() => Message;
	}


	public class DoubleAmethystError(string msg1, LocationRange loc2, string msg2) : AmethystError(msg1)
	{
		public readonly LocationRange Location2 = loc2;
		public readonly string RawMessage2 = msg2;

		public override CompilerMessager Display(IFileHandler handler, LocationRange loc, bool last = true)
		{
			var msg = base.Display(handler, loc, false);

			msg.AddContent("");
			msg.AddContent($"[green]{RawMessage2} {Location2.Start}[/]");
			msg.AddCode(handler, Location2, last);

			return msg;

			//AnsiConsole.MarkupLine($"[red]│\n│[/]   [green]{RawMessage2} {Location2.Start}[/]\n[red]│[/]");
			//AnsiConsole.MarkupLine("[red]│[/]   [turquoise2]" + lines[Location2.Start.Line - 1] + "[/]");
			//AnsiConsole.MarkupLine("[red]└─[/]  " + new string(' ', Location2.Start.Column - 1) + $"[violet]{new string('~', lastCol - Location2.Start.Column + 1)}[/]{(last ? "\n\n" : "")}");
		}
	}

	public class EmptyAmethystError() : AmethystError("")
	{
		public override CompilerMessager Display(IFileHandler handler, LocationRange loc, bool lastNewlines = true)
		{
			return new(Color.Red);
		}
	}
}
