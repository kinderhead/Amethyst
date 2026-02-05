using Amethyst.IR.Types;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode;
using Geode.Chains;
using Geode.IR;
using Geode.Values;
using System;

namespace Amethyst.IR.Instructions
{
	public class EntityRefInsn(ValueRef target) : Instruction([target])
	{
		public override string Name => "entity_ref";
		public override NBTType?[] ArgTypes => [NBTType.String];
		public override TypeSpecifier ReturnType => EntityType.Dummy;

		public override void Render(RenderContext ctx)
        {
            ctx.Macroize([Arg<ValueRef>(0).Expect()], (args, ctx) =>
            {
                var target = new NamedTarget(args[0].Value.ToString());

				ReturnValue.Expect<LValue>().Store(new LiteralValue(0), ctx);
				ctx.Add(new Execute().As(target).Run(new FunctionCommand("amethyst:core/entity/ref")));
				ctx.Add(new Execute().If.Entity(target).Run(ctx.WithFaux(ctx => ReturnValue.Expect<LValue>().Store(ctx.Func.GetFunctionReturnValue(ReturnType, 1), ctx)).Single()));
			});
        }

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;
	}
}
