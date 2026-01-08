using System;

namespace Geode.IR.Instructions
{
    public interface ILoadInsn : IBasicInstruction
    {
        public ValueRef Variable { get; }
    }
}
