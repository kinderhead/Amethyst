using Antlr4.Runtime;
using Geode;
using Geode.Errors;
using Spectre.Console;

namespace Amethyst.AST
{
	public class ParserErrorHandler(string path, string file, Visitor visitor) : BaseErrorListener, IAntlrErrorListener<int>, IFileHandler
	{
		public readonly string Path = path;
		public readonly string File = file;
		public readonly Visitor Visitor = visitor;
		public bool Errored { get; private set; } = false;

		public string GetFile(string path) => path == Path ? File : throw new FileNotFoundException(path);
		public string MapToPath(string mappedPath) => mappedPath;
		public string PathToMap(string path) => path;

		public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) => ThrowError(line, charPositionInLine, offendingSymbol.Text.Length, msg);
		public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) => ThrowError(line, charPositionInLine, offendingSymbol, msg);

		private void ThrowError(int line, int charPositionInLine, int length, string msg)
		{
			Errored = true;

			try
			{
				var loc = Visitor.LocOffset(new Location(Path, line, charPositionInLine + 1));
				var term = new CompilerMessager(Color.Red);
				term.Header($"Error at [underline]{loc}[/]");
				term.AddCode(this, new(loc, new(loc.File, loc.Line, loc.Column + length - 1)));
				term.Final().AddContent($"Syntax error: [turquoise2]{msg.EscapeMarkup()}[/]");

				Console.Out.Flush(); // Weird
			}
			catch (Exception e)
			{
				AnsiConsole.MarkupLine($"[red]Error displaying error: {e.Message}[/]");
			}
		}
	}
}
