using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public interface INegatable<T>
    {
        public Negatable<T> Negate();
    }
}
