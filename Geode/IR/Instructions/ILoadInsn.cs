using System;

namespace Geode.IR.Instructions
{
    public interface ILoadInsn : IBasicInsn
    {
        public ValueRef Variable { get; }
    }
}
