using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib.Utils
{
    public interface IToPointer : IPointerable
    {
        public IPointer ToPointer();
    }
}
