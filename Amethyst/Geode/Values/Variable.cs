using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;

namespace Amethyst.Geode.Values
{
	public class Variable(string name, string loc, TypeSpecifier type) : StackValue(-1, loc, type)
	{
		public readonly string Name = name;
		public override string ToString() => Name;

		private Variable? pointer = null;
		public Variable ToReference(FunctionContext ctx)
		{
			if (pointer is not null)
			{
				return pointer;
			}

			pointer = ctx.RegisterLocal($"${Name}_ref", new ReferenceTypeSpecifier(Type));
			ctx.Add(new StoreInsn(pointer, ctx.Add(new ReferenceInsn(this))));

			return pointer;
		}
	}
}