using System;

namespace Geode.IR.Instructions
{
    public interface IStoreInsn : IBasicInsn
    {
        public ValueRef Value { get; }
        public ValueRef Variable { get; }
    }
}
