using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Values;

namespace Amethyst.IR
{
	public static class ValueExtensions
	{
		extension(Variable self)
		{
			public Variable ToReference(FunctionContext ctx)
			{
				if (self.Pointer is not null)
				{
					return self.Pointer;
				}

				self.Pointer = ctx.RegisterLocal($"${self.Name}_ref", new ReferenceType(self.Type), ctx.LocationStack.Peek());
				ctx.Add(new StoreInsn(self.Pointer, ctx.Add(new ReferenceInsn(self))));

				return self.Pointer;
			}
		}

		extension (IValueLike self)
		{
			public bool IsTypeOrRef<T>() where T : TypeSpecifier => self.Type is T || (self.Type is ReferenceType ptr && ptr.Inner is T);
		}
	}
}
