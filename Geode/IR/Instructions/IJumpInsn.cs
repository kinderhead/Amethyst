using System;

namespace Geode.IR.Instructions
{
    public interface IJumpInsn : IBasicInstruction
    {
        public Block DestBlock { get; }
    }
}
