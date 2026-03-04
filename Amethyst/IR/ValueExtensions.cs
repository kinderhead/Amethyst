using Amethyst.Errors;
using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
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
			public T TypeOrRef<T>() where T : TypeSpecifier
			{
				if (self.Type is T ret1)
				{
					return ret1;
				}
				else if (self.Type is ReferenceType ptr && ptr.Inner is T ret2)
				{
					return ret2;
				}

				throw new InvalidTypeError(self.Type.ToString());
			}

			public bool IsTypeOrRef<T>() where T : TypeSpecifier => self.Type is T || (self.Type is ReferenceType ptr && ptr.Inner is T);
			public bool IsTypeOrRef<T>(out T type) where T : TypeSpecifier 
			{
				if (self.Type is T ret1)
				{
					type = ret1;
					return true;
				}
				else if (self.Type is ReferenceType ptr && ptr.Inner is T ret2)
				{
					type = ret2;
					return true;
				}

				type = null!;
				return false;
			}

			public IValue AsRef()
			{
				if (self.Type is ReferenceType)
				{
					return self.Expect();
				}

				return WeakReferenceType.From(self.Expect<DataTargetValue>());
			}
		}
	}
}
