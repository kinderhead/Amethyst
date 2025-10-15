using Amethyst.Errors;
using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST
{
	public abstract class AbstractTypeSpecifier(LocationRange loc) : Node(loc)
	{
		public TypeSpecifier Resolve(FunctionContext ctx, bool allowAuto = false) => Resolve((Compiler)ctx.Compiler, ctx.Decl.ID.GetContainingFolder(), allowAuto);

		public TypeSpecifier Resolve(Compiler ctx, string baseNamespace, bool allowAuto = false)
		{
			TypeSpecifier? ret = null;
			if (!ctx.WrapError(Location, () => ret = ResolveImpl(ctx, baseNamespace, allowAuto)))
			{
				throw new EmptyGeodeError();
			}

			return ret!;
		}

		protected abstract TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false);
	}

	public class SimpleAbstractTypeSpecifier(LocationRange loc, string type) : AbstractTypeSpecifier(loc)
	{
		public readonly string Type = type;

		protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false)
		{
			switch (Type)
			{
				case "void":
					return new VoidType();
				case "var":
					if (allowAuto)
					{
						return new VarType();
					}
					else
					{
						throw new InvalidTypeError(Type);
					}

				case "bool":
					return new PrimitiveType(NBTType.Boolean);
				case "byte":
					return new PrimitiveType(NBTType.Byte);
				case "short":
					return new PrimitiveType(NBTType.Short);
				case "int":
					return new PrimitiveType(NBTType.Int);
				case "long":
					return new PrimitiveType(NBTType.Long);
				case "float":
					return new PrimitiveType(NBTType.Float);
				case "double":
					return new PrimitiveType(NBTType.Double);
				case "string":
					return new PrimitiveType(NBTType.String);
				case "list":
					return new PrimitiveType(NBTType.List);
				case "nbt":
					return new PrimitiveType(NBTType.Compound);
				default:
					if (Type.Contains(':'))
					{
						if (ctx.IR.Types.TryGetValue(Type, out var ret))
						{
							return ret.Type;
						}
					}
					else if (GeodeBuilder.NamespaceWalk(baseNamespace, Type, ctx.IR.Types) is GlobalTypeSymbol sym)
					{
						return sym.Type;
					}

					break;
			}

			throw new UnknownTypeError(Type);
		}
	}

	public class AbstractListTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
	{
		public readonly AbstractTypeSpecifier Inner = inner;

		protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false) => new ListType(Inner.Resolve(ctx, baseNamespace));
	}

	public class AbstractReferenceTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
	{
		public readonly AbstractTypeSpecifier Inner = inner;

		protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false) => new ReferenceType(Inner.Resolve(ctx, baseNamespace));
	}

	public class AbstractWeakReferenceTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
	{
		public readonly AbstractTypeSpecifier Inner = inner;

		protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false) => new WeakReferenceType(Inner.Resolve(ctx, baseNamespace));
	}
}
