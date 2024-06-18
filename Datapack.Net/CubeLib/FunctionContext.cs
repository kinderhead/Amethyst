using Datapack.Net.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public struct FunctionContext
    {
        public MCFunction Target;
        public MCFunction Cleanup;
        public DeclareMCAttribute Attributes;
    }
}
