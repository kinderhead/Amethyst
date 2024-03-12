using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public class Score(string name, string criteria)
    {
        public readonly string Name = name;
        public readonly string Criteria = criteria;

        public override string ToString()
        {
            return Name;
        }
    }
}
