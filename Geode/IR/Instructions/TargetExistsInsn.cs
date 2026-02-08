using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class TargetExistsInsn(ValueRef target) : Instruction([target])
	{
		public override string Name => "target_exists";
		public override NBTType?[] ArgTypes => [NBTType.String];
		public override TypeSpecifier ReturnType => PrimitiveType.Bool;

		public override void Render(RenderContext ctx)
		{
			var ret = ReturnValue.Expect<LValue>();

			ret.Store(new LiteralValue(false), ctx);

			ctx.Macroize([Arg<ValueRef>(0).Expect()], (args, ctx) =>
			{
				var set = ctx.WithFaux(ctx => ret.Store(new LiteralValue(true), ctx)).Single();
				ctx.Add(new Execute().If.Entity(new NamedTarget(args[0].Value.Build())).Run(set));
			});
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;
	}
}
