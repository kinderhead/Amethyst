using Amethyst.AST;
using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Codegen.IR
{
	public class PopulateInstruction(LocationRange loc, StorageValue dest, Dictionary<string, Value> values) : Instruction(loc)
	{
		public readonly StorageValue Dest = dest;
		public readonly Dictionary<string, Value> Values = values;

		public override void Build()
		{
			var consts = new NBTCompound();
			foreach (var i in Values)
			{
				if (i.Value is LiteralValue v) consts[i.Key] = v.Value;
				else Add(i.Value.ReadTo(Dest.Storage, Dest.Path + $".{i.Key}"));
			}
			if (consts.Count != 0) Prepend(new DataCommand.Modify(Dest.Storage, Dest.Path).Set().Value(consts.ToString()));
		}
	}
}
