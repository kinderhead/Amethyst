using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Errors;
using Geode.Types;

namespace Geode.Values
{
	public class TargetSelectorValue(TargetType type, Dictionary<string, Value> args) : Value, IDataWritable, IAdvancedMacroValue
    {
        public readonly TargetType TargetType = type;
        public readonly Dictionary<string, Value> Arguments = args;

		public override TypeSpecifier Type => new TargetSelectorType();

		public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidTypeError(Type.ToString(), "int");
        public override bool Equals(object? obj) => ReferenceEquals(this, obj);
        public override int GetHashCode() => base.GetHashCode(); // Maybe do proper comparisons

        public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0)
        {
            foreach (var (k, v) in Arguments)
            {
                if (v is not IConstantValue)
                {
                    // This shouldn't happen, but just in case
                    throw new TargetSelectorMacroArgumentError(k);
                }
            }

            return cmd.If.Entity(new NamedTarget(ToString()));
        }
        
        public override FormattedText Render(FormattedText text, RenderContext ctx) => throw new NotImplementedException();

		public override string ToString()
        {
            var target = new Dictionary<string, string>();

            foreach (var (k, v) in Arguments)
            {
                target[k] = v.ToString();
            }

            return $"{TargetSelector.GetTypeName(TargetType)}{(target.Count > 0 ? $"[{TargetSelector.CompileDict(target)}]" : "")}";
        }

        public IConstantValue Macroize(Func<Value, IConstantValue> apply)
        {
            var newArgs = new Dictionary<string, Value>();

            foreach (var (k, v) in Arguments)
            {
                newArgs[k] = (Value)apply(v);
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
