using Amethyst.AST;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
			Add(Apply(Dest, Values));
		}

		public static List<Command> Apply(StorageValue dest, Dictionary<string, Value> values)
		{
			var cmds = Apply(dest, values, out var actual, out var macro);
			if (actual.AsConstant() is NBTCompound c) return [new DataCommand.Modify(dest.Storage, dest.Path, macro).Set().Value(c.ToString())];
			return cmds;
		}

		public static List<Command> Apply(StorageValue dest, Dictionary<string, Value> values, out Value actual, out bool macro)
		{
			var consts = new NBTCompound();
			var cmds = new List<Command>();
			macro = false;
			foreach (var i in values)
			{
				if (i.Value.IsMacro) macro = true;
				if (i.Value.AsConstant() is NBTValue c) consts[i.Key] = c;
				else cmds.Add(i.Value.ReadTo(dest.Storage, dest.Path + $".{i.Key}"));
			}
			if (consts.Count != 0) cmds.Insert(0, new DataCommand.Modify(dest.Storage, dest.Path, macro).Set().Value(consts.ToString()));

			if (consts.Count == values.Count && values.Count != 0)
			{
				actual = new LiteralValue(consts);
				return [];
			}
			else
			{
				actual = dest;
				return cmds;
			}
		}
	}

	public class PopulateListInstruction(LocationRange loc, StorageValue dest, List<Value> values) : Instruction(loc)
	{
		public readonly StorageValue Dest = dest;
		public readonly List<Value> Values = values;

		public override void Build()
		{
			bool onConstants = true;
			var consts = new NBTList();
			for (int i = 0; i < Values.Count; i++)
			{
				if (onConstants)
				{
					if (Values[i] is not LiteralValue v)
					{
						onConstants = false;
						if (consts.Count != 0) Add(new DataCommand.Modify(Dest.Storage, Dest.Path).Set().Value(consts.ToString()));
						Add(Values[i].AppendTo(Dest.Storage, Dest.Path));
						continue;
					}
					else consts.Add(v.Value);
				}
				else Add(Values[i].AppendTo(Dest.Storage, Dest.Path));
			}

			if (onConstants) Add(new DataCommand.Modify(Dest.Storage, Dest.Path).Set().Value(consts.ToString()));
		}
	}
}
