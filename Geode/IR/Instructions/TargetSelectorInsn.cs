using Datapack.Net.Data;
using Datapack.Net.Function;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	// SortedDictionary for the same reason as CompoundInsn
	public class TargetSelectorInsn(TargetType type, SortedDictionary<string, ValueRef> vals) : Instruction(vals.Values)
	{
		public override string Name => "target";
		public override NBTType?[] ArgTypes => [.. Enumerable.Repeat<NBTType?>(null, Arguments.Length)];
		public override TypeSpecifier ReturnType => new TargetSelectorType();

		public readonly TargetType Type = type;
		public readonly string[] Keys = [.. vals.Keys];

		public override void Render(RenderContext ctx) { }

		protected override IValue? ComputeReturnValue(FunctionContext ctx)
		{
			var args = new Dictionary<string, IValue>();

			for (var i = 0; i < Arguments.Length; i++)
			{
				var val = Arg<ValueRef>(i);
				ReturnValue.Dependencies.Add(val);
				args[Keys[i]] = val.Expect();
			}

			return new TargetSelectorValue(Type, args);
		}
	}
}
