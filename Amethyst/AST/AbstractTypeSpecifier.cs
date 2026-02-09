using Amethyst.Errors;
using Amethyst.IR.Types;
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
			if (!ctx.WrapError(Location, [System.Diagnostics.DebuggerNonUserCode] () => ret = ResolveImpl(ctx, baseNamespace, allowAuto)))
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
					else if (ctx.IR.Types.TryGetValue($"minecraft:{Type}", out var mc))
					{
						return mc.Type;
					}
					else if (ctx.IR.Types.TryGetValue($"builtin:{Type}", out var bt))
					{
						return bt.Type;
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

	public class AbstractMapTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
	{
		public readonly AbstractTypeSpecifier Inner = inner;

		protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false) => new SimpleMapType(Inner.Resolve(ctx, baseNamespace));
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
