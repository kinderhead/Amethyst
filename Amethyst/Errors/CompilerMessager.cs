using Amethyst.AST;
using Spectre.Console;

namespace Amethyst.Errors
{
	public class CompilerMessager(Color baseColor)
	{
		public const int PADDING = 4;
		public const int MAX_CODE_LINES = 3;

		public readonly Color BaseColor = baseColor;
		public bool ShouldBeFinal = false;

		public Style Style => new(BaseColor);

		public CompilerMessager Header(string markup)
		{
			AnsiConsole.Write(new Markup(markup + "\n", Style));
			return this;
		}

		public CompilerMessager AddContent(string markup)
		{
			AnsiConsole.Write(new Markup(
				(ShouldBeFinal ? "└─" + new string(' ', PADDING - 2) : "│" + new string(' ', PADDING - 1))
				+ markup +
				(ShouldBeFinal ? "\n\n\n" : "\n"), Style
			));

			return this;
		}

		public CompilerMessager AddCode(IFileHandler handler, LocationRange loc, bool final = false)
		{
			_ = AddContent("");

			if (!handler.Files.TryGetValue(loc.Start.File, out var file))
			{
				_ = AddContent("[turquoise2]<builtin>[/]");
				_ = AddContent("");
				return this;
			}

			// Evil tabs
			string[] lines = [.. file.Split('\n').Skip(loc.Start.Line - 1).Take(Math.Min(loc.End.Line - loc.Start.Line + 1, MAX_CODE_LINES)).Select(i => i.Replace('\t', ' '))];

			var indentToSkip = lines.Select(i => i.TakeWhile(c => c == ' ').Count()).Min();

			foreach (var (idex, i) in lines.Index())
			{
				// Prevent double finals
				if (final && idex > 0 && idex == lines.Length - 1)
				{
					_ = Final();
				}

				_ = AddContent($"[turquoise2]{i[indentToSkip..].EscapeMarkup()}[/]");
			}

			if (lines.Length == 1)
			{
				if (final)
				{
					_ = Final();
				}

				_ = AddContent($"{new string(' ', loc.Start.Column - 1 - indentToSkip)}{new string('~', loc.End.Column - loc.Start.Column + 1)}");
			}
			else if (!final)
			{
				_ = AddContent("");
			}

			return this;
		}

		// Maybe do this by storing the lines and rendering them all at once
		public CompilerMessager Final()
		{
			ShouldBeFinal = true;
			return this;
		}
	}
}
