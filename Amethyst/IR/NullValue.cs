using Amethyst.IR.Types;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode;
using Geode.Types;
using Geode.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.IR
{
	public class NullValue : Value
	{
		public override TypeSpecifier Type => new ReferenceType(new VoidType(), false);
		public override ScoreValue AsScore(RenderContext ctx) => ctx.Builder.Constant(0);
		public override FormattedText Render(FormattedText text, RenderContext ctx) => text.Text("null");
	}
}
