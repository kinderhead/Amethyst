using Amethyst.Errors;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class ReturnInsn : Instruction
    {
        public ReturnInsn(ValueRef ret) : base([ret]) { }
        public ReturnInsn() : base([]) { }

        public override string Name => "ret";
        public override NBTType?[] ArgTypes => [null];
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();
		public override bool IsReturn => true;

        public override void Render(RenderContext ctx)
        {
            var val = Arguments.Length == 1 ? Arg<ValueRef>(0).Expect() : null;

            if (val is not null && val.Type != ctx.Func.Decl.FuncType.ReturnType) throw new InvalidTypeError(val.Type.ToString(), ctx.Func.Decl.FuncType.ReturnType.ToString());
            if (val is null && ctx.Func.Decl.FuncType.ReturnType is not VoidTypeSpecifier) throw new InvalidTypeError("void", ctx.Func.Decl.FuncType.ReturnType.ToString());

            var ret = ctx.Func.GetFunctionReturnValue();
            if (val is null) ret.Store(new LiteralValue(0), ctx);
            else ret.Store(val, ctx);
        }

        protected override Value? ComputeReturnValue() => new VoidValue();

		public override void CheckArguments()
		{
            if (Arguments.Length == 0) return;
            if (Arguments.Length != 1) throw new MismatchedArgumentCountError(1, Arguments.Length);
            return;
		}
    }
}
