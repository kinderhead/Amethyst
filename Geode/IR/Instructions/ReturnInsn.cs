using Datapack.Net.Data;
using Geode.Errors;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class ReturnInsn : Instruction, IBlockCapstoneInsn
	{
		public ReturnInsn(ValueRef ret) : base([ret]) { }
		public ReturnInsn() : base([]) { }

		public override string Name => "ret";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => new VoidType();
		public override bool IsReturn => true;

		public override void Render(RenderContext ctx)
		{
			var val = Arguments.Length == 1 ? Arg<ValueRef>(0).Expect() : null;

			if (val is null && ctx.Func.Decl.FuncType.ReturnType is not VoidType)
			{
				throw new InvalidTypeError("void", ctx.Func.Decl.FuncType.ReturnType.ToString());
			}

			if (ctx.Block != ctx.Func.Start)
			{
				ctx.Func.GetIsFunctionReturningValue().Store(new LiteralValue(true), ctx);
			}

			if (val is not null)
			{
				// Make SetType affect existing values in ValueRef
				if (val is IConstantValue c && Arg<ValueRef>(0).Type.WrapInQuotesForMacro)
				{
					ctx.Func.GetFunctionReturnValue().Store(new LiteralValue($"{c.Value}"), ctx);
				}
				else
				{
					ctx.Func.GetFunctionReturnValue().Store(val, ctx);
				}
			}
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();

		public override void CheckArguments()
		{
			if (Arguments.Length > 2)
			{
				throw new MismatchedArgumentCountError(1, Arguments.Length);
			}

			return;
		}
	}
}
