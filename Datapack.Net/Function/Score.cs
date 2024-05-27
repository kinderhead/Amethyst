using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public class Score(string name, string criteria, string displayName = "")
    {
        public readonly string Name = name;
        public readonly string Criteria = criteria;
        public readonly string DisplayName = displayName;

        public override string ToString()
        {
            return Name;
        }

        public static bool operator ==(Score? a, Score? b) => a?.Name == b?.Name;
        public static bool operator !=(Score? a, Score? b) => a?.Name != b?.Name;

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is Score s) return this == s;
            return obj.Equals(this);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
