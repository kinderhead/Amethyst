using System;

namespace Geode.IR.Instructions
{
    public interface IStoreInsn : IBasicInstruction
    {
        public ValueRef Value { get; }
        public ValueRef Variable { get; }
    }
}
