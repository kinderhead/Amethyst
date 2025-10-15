using Datapack.Net.Data;
using Datapack.Net.Function;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
    // SortedDictionary for the same reason as CompoundInsn
    public class TargetSelectorInsn(TargetType type, SortedDictionary<string, ValueRef> vals) : Instruction(vals.Values)
    {
        public override string Name => "target";
        public override NBTType?[] ArgTypes => [.. Enumerable.Repeat<NBTType?>(null, Arguments.Length)];
        public override TypeSpecifier ReturnType => new TargetSelectorType();

        public readonly TargetType Type = type;
        public readonly string[] Keys = [.. vals.Keys];

        public override void Render(RenderContext ctx)
        {
            ctx.Builder.Macroizer.Run(ctx, Arguments.Cast<ValueRef>(), (args, ctx) =>
            {
                var target = new Dictionary<string, string>();

                for (var i = 0; i < args.Length; i++)
                {
                    target[Keys[i]] = args[i].Value.Build();
                }

                ReturnValue.Expect<DataTargetValue>().Store(new LiteralValue($"{TargetSelector.GetTypeName(Type)}{(args.Length > 0 ? $"[{TargetSelector.CompileDict(target)}]" : "")}", ReturnType), ctx);
            });
        }

        protected override Value? ComputeReturnValue(FunctionContext ctx)
        {
            var args = new Dictionary<string, string>();

            for (var i = 0; i < Arguments.Length; i++)
            {
                if (Arg<ValueRef>(i).Expect() is not IConstantValue l)
                {
                    return null;
                }

                args[Keys[i]] = l.Value.Build();
            }

            return new LiteralValue($"{TargetSelector.GetTypeName(Type)}{(args.Count > 0 ? $"[{TargetSelector.CompileDict(args)}]" : "")}", ReturnType);
        }
    }
}
