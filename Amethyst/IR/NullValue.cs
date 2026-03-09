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
	public class NullValue : StoreableValue
	{
		public override TypeSpecifier Type => new ReferenceType(new VoidType(), false);
		public override ScoreValue AsScore(RenderContext ctx) => ctx.Builder.Constant(0);
		public override IValue AsStoreable() => Type.DefaultValue;
		public override FormattedText Render(FormattedText text, RenderContext ctx) => text.Text("null");
		public override string ToString() => "null";
	}
}
