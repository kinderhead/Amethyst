namespace Amethyst.Geode.IR.InlineFunctions
{
    public class ListAdd() : FunctionValue("minecraft:add", new FunctionTypeSpecifier(AST.FunctionModifiers.None, new VoidTypeSpecifier(), [
        new(AST.ParameterModifiers.None, new ListTypeSpecifier(new GenericTypeSpecifier("T")), "this"),
        new(AST.ParameterModifiers.None, new GenericTypeSpecifier("T"), "value")
    ]))
    {
        public override void Call(RenderContext ctx, IEnumerable<ValueRef> args)
        {
            throw new NotImplementedException();
        }
    }
}
