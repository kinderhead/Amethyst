using Spectre.Console;

namespace Geode.Errors
{
	public class GeodeError(string msg) : Exception
	{
		public readonly string RawMessage = msg;

		public override string Message => $"{GetType().Name}: {RawMessage}";

		public virtual CompilerMessager Display(IFileHandler handler, LocationRange loc, bool last = true)
		{
			// Console.WriteLine(StackTrace);

			var msg = new CompilerMessager(Color.Red);
			msg.Header($"Error at {loc.Start}");
			msg.AddCode(handler, loc);

			if (last)
			{
				msg.Final();
			}

			msg.AddContent($"{GetType().Name}: [yellow]{RawMessage.EscapeMarkup()}[/]");

			return msg;
		}

		public override string ToString() => Message;
	}

	public class DoubleGeodeError(string msg1, LocationRange loc2, string msg2) : GeodeError(msg1)
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
		}
	}

	public class EmptyGeodeError() : GeodeError("")
	{
		public override CompilerMessager Display(IFileHandler handler, LocationRange loc, bool lastNewlines = true) => new(Color.Red);
	}
}
