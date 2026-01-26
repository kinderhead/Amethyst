using System;

namespace Geode.IR.Instructions
{
    public interface IJumpInsn : IBlockCapstoneInsn
    {
        public Block DestBlock { get; }
    }
}
