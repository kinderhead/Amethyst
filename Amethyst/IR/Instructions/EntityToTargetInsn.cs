using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.IR.Instructions
{
	public class EntityToTargetInsn(ValueRef entity) : Instruction([entity])
	{
		public override string Name => "entity_to_target";
		public override NBTType?[] ArgTypes => [NBTType.Int];
		public override TypeSpecifier ReturnType => new TargetSelectorType();

		public override void Render(RenderContext ctx)
		{
			ReturnValue.Expect<DynamicValue>()
				.Add("@e[scores={amethyst_id=")
				.Add(Arg<ValueRef>(0).Expect())
				.Add("}]");
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx)
		{
			var entity = Arg<ValueRef>(0);
			ReturnValue.AddDependency(entity);
			return new DynamicValue(ReturnType);
		}
	}
}
