using Datapack.Net.Function.Commands;
using Datapack.Net.Pack;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public class MCFunction(NamespacedID id, bool partial = false) : Resource(id)
    {
        public bool Macro { get; protected set; }
        public readonly bool Partial = partial;

        internal List<Command> Commands = [];

        public int Length => Commands.Count;

        public void Add(Command command)
        {
            Commands.Add(command);
            if (command.Macro) Macro = true;
        }

		public void Add(params Command[] commands)
		{
			foreach (var i in commands)
			{
                Add(i);
			}
		}

		public void Prepend(Command command)
        {
            Commands.Insert(0, command);
            if (command.Macro) Macro = true;
        }

        public override string Build(DP pack) => Build();

        public string Build()
        {
            StringBuilder sb = new();
            foreach (var i in Commands)
            {
                sb.AppendLine(i.Build());
                if (i is ReturnCommand) break;
            }
            return sb.ToString();
        }

        public bool SameContents(MCFunction other) => Build() == other.Build();

        public static bool operator==(MCFunction a, MCFunction b) => a.ID == b.ID;
        public static bool operator!=(MCFunction a, MCFunction b) => a.ID != b.ID;

        public override bool Equals(object? obj)
        {
            if (obj is MCFunction f) return this == f;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}
