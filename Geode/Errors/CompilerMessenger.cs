using Spectre.Console;

namespace Geode.Errors
{
    public class CompilerMessenger(Color baseColor)
    {
        public const int PADDING = 4;
        public const int MAX_CODE_LINES = 3;

        public readonly Color BaseColor = baseColor;
        public bool ShouldBeFinal;

        public Style Style => new(BaseColor);

        public CompilerMessenger Header(string markup)
        {
            AnsiConsole.Write(new Markup(markup + "\n", Style));
            return this;
        }

        public CompilerMessenger AddContent(string markup)
        {
            AnsiConsole.Write(new Markup(
                (ShouldBeFinal ? "└─" + new string(' ', PADDING - 2) : "│" + new string(' ', PADDING - 1))
                + markup +
                (ShouldBeFinal ? "\n\n\n" : "\n"), Style
            ));

            return this;
        }

        public CompilerMessenger AddCode(IFileHandler handler, LocationRange loc, bool final = false)
        {
            AddContent("");

            if (!handler.GetFileOrNull(loc.Start.File, out var file))
            {
                AddContent("[turquoise2]<builtin>[/]");
                AddContent("");
                return this;
            }

            // Evil tabs
            string[] lines =
            [
                .. file.Split('\n').Skip(loc.Start.Line - 1)
                       .Take(Math.Min(loc.End.Line - loc.Start.Line + 1, MAX_CODE_LINES)).Select(i => i.Replace('\t', ' '))
            ];

            var indentToSkip = lines.Select(i => i.TakeWhile(c => c == ' ').Count()).Min();

            foreach (var (index, i) in lines.Index())
            {
                // Prevent double finals
                if (final && index > 0 && index == lines.Length - 1) Final();

                AddContent($"[turquoise2]{i[indentToSkip..].EscapeMarkup().TrimEnd()}[/]");
            }

            if (lines.Length == 1)
            {
                if (final) Final();

                AddContent($"{new string(' ', loc.Start.Column - 1 - indentToSkip)}{new string('~', loc.End.Column - loc.Start.Column + 1)}");
            }
            else if (!final) AddContent("");

            return this;
        }

        // Maybe do this by storing the lines and rendering them all at once
        public CompilerMessenger Final()
        {
            ShouldBeFinal = true;
            return this;
        }
    }
}