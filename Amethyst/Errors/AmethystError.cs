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
			//Console.WriteLine(StackTrace);

			var msg = new CompilerMessager(Color.Red);
			_ = msg.Header($"Error at {loc.Start}");
			_ = msg.AddCode(handler, loc);

			if (last)
			{
				_ = msg.Final();
			}

			_ = msg.AddContent($"{GetType().Name}: [yellow]{RawMessage.EscapeMarkup()}[/]");

			return msg;
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

			_ = msg.AddContent("");
			_ = msg.AddContent($"[green]{RawMessage2} {Location2.Start}[/]");
			_ = msg.AddCode(handler, Location2, last);

			return msg;
		}
	}

	public class EmptyAmethystError() : AmethystError("")
	{
		public override CompilerMessager Display(IFileHandler handler, LocationRange loc, bool lastNewlines = true) => new(Color.Red);
	}
}
