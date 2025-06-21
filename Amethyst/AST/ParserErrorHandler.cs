using Antlr4.Runtime;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST
{
	public class ParserErrorHandler(string path, string file) : BaseErrorListener, IAntlrErrorListener<int>
	{
		public readonly string Path = path;
		public readonly string[] Lines = file.Split("\n");
		public bool Errored { get; private set; } = false;

		public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) => ThrowError(line, charPositionInLine, msg);
		public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) => ThrowError(line, charPositionInLine, msg);

		private void ThrowError(int line, int charPositionInLine, string msg)
		{
			Errored = true;

			try
			{
				AnsiConsole.MarkupLine($"[red]Error at {Path} line {line}:[/]");
				AnsiConsole.MarkupLine("[red]" + Lines[line - 1].EscapeMarkup() + "[/]");
				AnsiConsole.MarkupLine(new string(' ', charPositionInLine) + "[yellow]^[/]");
				AnsiConsole.MarkupLine($"[red]Syntax error:[/] [turquoise2]{msg.EscapeMarkup()}[/]\n\n");
				Console.Out.Flush(); // Weird
			}
			catch (Exception e)
			{
				AnsiConsole.MarkupLine($"[red]Error displaying error: {e.Message}[/]");
			}
			finally
			{
				Environment.Exit(1);
			}
		}
	}
}
