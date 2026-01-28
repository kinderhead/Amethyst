using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Errors;
using Geode.Types;
using System;

namespace Geode.Values
{
	public class RangeValue(ValueRef min, ValueRef max, TypeSpecifier type) : Value, IDataWritable, IAdvancedMacroValue
	{
        public readonly ValueRef Min = min;
        public readonly ValueRef Max = max;
        public override TypeSpecifier Type => type;

		public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidTypeError(Type.ToString(), "int");
		public override void If(Action<Execute> apply, RenderContext ctx, int tmp = 0) => throw new InvalidTypeError(Type.ToString(), "bool");

		public IConstantValue Macroize(Func<IValue, IConstantValue> apply) => new LiteralValue(new NBTRawString($"{apply(Min.Expect()).Value}..{apply(Max.Expect()).Value}"), Type);

		public override FormattedText Render(FormattedText text, RenderContext ctx) => throw new NotImplementedException();

		public void StoreTo(DataTargetValue val, RenderContext ctx)
        {
            ctx.Builder.Macroizer.Run(ctx, [this], (args, ctx) =>
            {
                // Macroize returns NBTRawString, so make it a regular string to add quotes
                val.Store(new LiteralValue(args[0].Value.Build(), Type), ctx);
            });
        }
	}
}
