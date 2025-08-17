namespace Amethyst.Geode.IR.Instructions
{
    // public class DeclareInsn(Variable val) : Instruction([new ValueRef(val)])
    // {
    //     public override string Name => "decl";
    //     public override NBTType?[] ArgTypes => [null];
    //     public override TypeSpecifier ReturnType => new VoidTypeSpecifier();
    //     public Variable Value => (Variable?)Arg<ValueRef>(0).Value ?? throw new InvalidOperationException("Expected variable");
    //     protected override Value? ComputeReturnValue() => new VoidValue();

    //     public override string Dump(Func<IInstructionArg, string> valueMap) => $"decl {Value.Type} {valueMap(Arguments[0])}";
    // }
}
