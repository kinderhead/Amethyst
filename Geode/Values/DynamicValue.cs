using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Geode.Values
{
	public class DynamicValue(TypeSpecifier type) : Value, IDataWritable, IAdvancedMacroValue
	{
		public override TypeSpecifier Type => type;

		private readonly List<IValue> parts = [];

		public DynamicValue Add(IValue val)
		{
			parts.Add(val);
			return this;
		}

		public DynamicValue Add(string str) => Add(LiteralValue.Raw(str));

		public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();
		public override void If(Action<Execute> apply, RenderContext ctx, int tmp = 0) => throw new NotImplementedException();
		public override FormattedText Render(FormattedText text, RenderContext ctx) => throw new NotImplementedException();

		public void StoreTo(DataTargetValue val, RenderContext ctx)
		{
			ctx.Macroize([this], (args, ctx) =>
			{
				// Macroize returns NBTRawString, so make it a regular string to add quotes
				val.Store(new LiteralValue(args[0].Value.Build(), Type), ctx);
			});
		}

		public IConstantValue Macroize(Func<IValue, IConstantValue> apply)
		{
			var builder = new StringBuilder();

			foreach (var i in parts)
			{
				builder.Append(apply(i).Value);
			}

			return LiteralValue.Raw(builder.ToString());
		}
	}
}
