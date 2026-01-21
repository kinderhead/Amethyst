using System;

namespace Geode.IR.Instructions
{
    public interface IBranchInsn : IBasicInstruction
    {
        public Block[] Destinations { get; }
    }
}
