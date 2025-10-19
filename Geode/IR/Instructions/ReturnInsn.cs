using Datapack.Net.Data;
using Geode.Errors;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class ReturnInsn : Instruction
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

			if (val is not null && val.Type != ctx.Func.Decl.FuncType.ReturnType)
			{
				throw new InvalidTypeError(val.Type.ToString(), ctx.Func.Decl.FuncType.ReturnType.ToString());
			}

			if (val is null && ctx.Func.Decl.FuncType.ReturnType is not VoidType)
			{
				throw new InvalidTypeError("void", ctx.Func.Decl.FuncType.ReturnType.ToString());
			}

			if (ctx.Block != ctx.Func.FirstBlock)
			{
				ctx.Func.GetIsFunctionReturningValue().Store(new LiteralValue(true), ctx);
			}

			if (val is not null)
			{
				ctx.Func.GetFunctionReturnValue().Store(val, ctx);
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
