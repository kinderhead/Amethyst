using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public class Team(string name) : INegatable<Team>
    {
        public readonly string Name = name;

        public Negatable<Team> Negate()
        {
            return new(this, true);
        }

        public override string ToString()
        {
            return Name;
        }

        public static Negatable<Team> operator !(Team team) => team.Negate();
        public static implicit operator Negatable<Team>(Team team) => new(team);
    }
}
