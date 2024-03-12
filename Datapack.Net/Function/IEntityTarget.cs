using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public interface IEntityTarget
    {
        public string Get();
    }

    public class NamedTarget(string name) : IEntityTarget
    {
        public readonly string Name = name;

        public string Get()
        {
            return Name;
        }
    }
}
