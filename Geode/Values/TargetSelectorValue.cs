using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Errors;
using Geode.Types;
using Geode.Util;

namespace Geode.Values
{
	public class TargetSelectorValue(TargetType type, MultiDictionary<string, IValue> args) : Value, IDataWritable, IAdvancedMacroValue
	{
		public readonly TargetType TargetType = type;
		public readonly MultiDictionary<string, IValue> Arguments = args;

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
			var target = new MultiDictionary<string, string>();

			foreach (var (k, v) in Arguments)
			{
				var arg = k;
				var negated = false;

				if (k.StartsWith('!'))
                {
                    arg = k[1..];
					negated = true;
                }

				if (v is LiteralValue literal && literal.Is<NBTString>(out var str) && str.Value.Contains('!'))
				{
					throw new TargetSelectorNegatedLiteralError(str.Value);
				}

				string val;

				// Remove quotes if constant
				if ((arg is "type" || v.Type is RangeType) && v is IConstantValue c && c.Value is NBTString str2)
				{
					val = str2.Value;
				}
				else
				{
					val = v.ToString() ?? "";
				}

				if (negated)
                {
                    target.Add(arg, '!' + val);
                }
				else
                {
					target.Add(arg, val);
				}
			}

			return $"{TargetSelector.GetTypeName(TargetType)}{(target.Count > 0 ? $"[{TargetSelector.CompileDict(target)}]" : "")}";
		}

		public IConstantValue Macroize(Func<IValue, IConstantValue> apply)
		{
			var newArgs = new MultiDictionary<string, IValue>();

			foreach (var (k, v) in Arguments)
			{
				if (v is DataTargetValue nbt && v.Type == PrimitiveType.String)
				{
					// Ignore macro string warnings
					newArgs.Add(k, apply(new RawDataTargetValue(nbt.Target.GetTarget(), PrimitiveType.Compound)));
				}
				else
				{
					newArgs.Add(k, apply(v));
				}
			}

			return new LiteralValue(new NBTRawString(new TargetSelectorValue(TargetType, newArgs).ToString()), Type);
		}

		public void StoreTo(DataTargetValue val, RenderContext ctx)
		{
			foreach (var i in Arguments["name"].Concat(Arguments["!name"]))
			{
				if (i is not IConstantValue)
                {
					throw new TargetSelectorMacroArgumentError("name");
				}
			}

			foreach (var i in Arguments["nbt"].Concat(Arguments["!nbt"]))
			{
				if (i is not IConstantValue)
				{
					throw new TargetSelectorMacroArgumentError("nbt");
				}
			}

			ctx.Builder.Macroizer.Run(ctx, [this], (args, ctx) =>
			{
				// Macroize returns NBTRawString, so make it a regular string to add quotes
				val.Store(new LiteralValue(args[0].Value.Build(), Type), ctx);
			});
		}
	}
}
