using System;

namespace Geode.IR.Instructions
{
    public interface IBranchInsn : IBasicInstruction
    {
        public Block TrueBlock { get; }
        public Block FalseBlock { get; }
    }
}
