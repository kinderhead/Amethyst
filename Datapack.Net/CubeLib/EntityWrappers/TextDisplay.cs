using Datapack.Net.Data;
using Datapack.Net.Data._1_20_4;
using Datapack.Net.Function;

namespace Datapack.Net.CubeLib.EntityWrappers
{
	public class TextDisplay(ScoreRef id) : Display(id)
	{
		public override EntityType Type => Entities.TextDisplay;

		public StringEntityProperty RawText { get => GetAs<StringEntityProperty>("text"); set => value.Set(this, "text"); }

		public void SetText(FormattedText text)
		{
			if (text.HasHoverOrClickEvents)
			{
				throw new ArgumentException("Formatted text has hover or click events in invalid places");
			}

			RawText = text.ToString();
		}
	}
}
