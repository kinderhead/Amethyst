using Datapack.Net.Data;

namespace Geode.IR.Instructions
{
    // SortedDictionary for the same reason as CompoundInsn
    public class TargetSelectorInsn(SortedDictionary<string, ValueRef> vals) : Instruction(vals.Values)
    {
        public override string Name => "target";
        public override NBTType?[] ArgTypes => [.. Enumerable.Repeat<NBTType?>(null, Arguments.Length)];
        public override TypeSpecifier ReturnType => throw new NotImplementedException();

        public override void Render(RenderContext ctx) => throw new NotImplementedException();
        protected override Value? ComputeReturnValue(FunctionContext ctx) => throw new NotImplementedException();
    }
}
