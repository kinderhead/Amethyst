using System;

namespace Geode.IR.Instructions
{
    public interface IBranchInsn : IBlockCapstoneInsn
    {
        public Block[] Destinations { get; }
    }
}
