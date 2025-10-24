using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Errors;
using Geode.Types;

namespace Geode.Values
{
	public class TargetSelectorValue(TargetType type, Dictionary<string, IValue> args) : Value, IDataWritable, IAdvancedMacroValue
	{
		public readonly TargetType TargetType = type;
		public readonly Dictionary<string, IValue> Arguments = args;

		public override TypeSpecifier Type => new TargetSelectorType();

		public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidTypeError(Type.ToString(), "int");

		public override void If(Action<Execute> apply, RenderContext ctx, int tmp = 0) => ctx.Builder.Macroizer.Run(ctx, [this], (args, ctx) =>
																								   {
																									   var cmd = new Execute().If.Entity(new NamedTarget(args[0].Value.Build()));
																									   apply(cmd);
																									   ctx.Add(cmd);
																								   });

		public override FormattedText Render(FormattedText text, RenderContext ctx) => throw new NotImplementedException();

		public override string ToString()
		{
			var target = new Dictionary<string, string>();

			foreach (var (k, v) in Arguments)
			{
				// Remove quotes if constant
				if (k is "type" && v is IConstantValue c && c.Value is NBTString str)
				{
					target[k] = str.Value;
				}
				else
				{
					target[k] = v.ToString() ?? "";
				}
			}

			return $"{TargetSelector.GetTypeName(TargetType)}{(target.Count > 0 ? $"[{TargetSelector.CompileDict(target)}]" : "")}";
		}

		public IConstantValue Macroize(Func<IValue, IConstantValue> apply)
		{
			var newArgs = new Dictionary<string, IValue>();

			foreach (var (k, v) in Arguments)
			{
				if (v is DataTargetValue nbt && v.Type == PrimitiveType.String)
				{
					// Ignore macro string warnings
					newArgs[k] = apply(new RawDataTargetValue(nbt.Target.GetTarget(), PrimitiveType.Compound));
				}
				else
				{
					newArgs[k] = apply(v);
				}
			}

			return new LiteralValue(new NBTRawString(new TargetSelectorValue(TargetType, newArgs).ToString()), Type);
		}

		public void StoreTo(DataTargetValue val, RenderContext ctx)
		{
			if (Arguments.TryGetValue("name", out var name) && name is not IConstantValue)
			{
				throw new TargetSelectorMacroArgumentError("name");
			}
			else if (Arguments.TryGetValue("nbt", out var nbt) && nbt is not IConstantValue)
			{
				throw new TargetSelectorMacroArgumentError("nbt");
			}

			ctx.Builder.Macroizer.Run(ctx, [this], (args, ctx) =>
			{
				// Macroize returns NBTRawString, so make it a regular string to add quotes
				val.Store(new LiteralValue(args[0].Value.Build(), Type), ctx);
			});
		}
	}
}
