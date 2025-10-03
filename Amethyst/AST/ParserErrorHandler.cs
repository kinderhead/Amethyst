using Amethyst.Errors;
using Antlr4.Runtime;
using Spectre.Console;

namespace Amethyst.AST
{
	public class ParserErrorHandler(string path, string file, Visitor visitor) : BaseErrorListener, IAntlrErrorListener<int>, IFileHandler
	{
		public readonly string Path = path;
		public readonly Visitor Visitor = visitor;
		public bool Errored { get; private set; } = false;

		public Dictionary<string, string> Files => new([new KeyValuePair<string, string>(Path, file)]);

		public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) => ThrowError(line, charPositionInLine, offendingSymbol.Text.Length, msg);
		public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) => ThrowError(line, charPositionInLine, offendingSymbol, msg);

		private void ThrowError(int line, int charPositionInLine, int length, string msg)
		{
			Errored = true;

			try
			{
				//AnsiConsole.MarkupLine($"[red]Error at {Path} line {line}:[/]");
				//AnsiConsole.MarkupLine("[red]" + Lines[line - 1].EscapeMarkup() + "[/]");
				//AnsiConsole.MarkupLine(new string(' ', charPositionInLine) + "[yellow]^[/]");
				//AnsiConsole.MarkupLine($"[red]Syntax error:[/] [turquoise2]{msg.EscapeMarkup()}[/]\n\n");

				var loc = Visitor.LocOffset(new Location(Path, line, charPositionInLine + 1));
				var term = new CompilerMessager(Color.Red);
				_ = term.Header($"Error at {loc}");
				_ = term.AddCode(this, new(loc, new(loc.File, loc.Line, loc.Column + length - 1)));
				_ = term.Final().AddContent($"Syntax error: [turquoise2]{msg.EscapeMarkup()}[/]");

				Console.Out.Flush(); // Weird
			}
			catch (Exception e)
			{
				AnsiConsole.MarkupLine($"[red]Error displaying error: {e.Message}[/]");
			}
		}
	}
}
