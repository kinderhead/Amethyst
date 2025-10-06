using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.Types;
using Datapack.Net.Data;
using Datapack.Net.Utils;

namespace Amethyst.AST
{
	public abstract class AbstractTypeSpecifier(LocationRange loc) : Node(loc)
	{
		public TypeSpecifier Resolve(FunctionContext ctx, bool allowAuto = false) => Resolve(ctx.Compiler, ctx.Decl.ID.GetContainingFolder(), allowAuto);

		public TypeSpecifier Resolve(Compiler ctx, string baseNamespace, bool allowAuto = false)
		{
			TypeSpecifier? ret = null;
			if (!ctx.WrapError(Location, () => ret = ResolveImpl(ctx, baseNamespace, allowAuto)))
			{
				throw new EmptyAmethystError();
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
					return new VoidTypeSpecifier();
				case "var":
					if (allowAuto)
					{
						return new VarTypeSpecifier();
					}
					else
					{
						throw new InvalidTypeError(Type);
					}

				case "bool":
					return new PrimitiveTypeSpecifier(NBTType.Boolean);
				case "byte":
					return new PrimitiveTypeSpecifier(NBTType.Byte);
				case "short":
					return new PrimitiveTypeSpecifier(NBTType.Short);
				case "int":
					return new PrimitiveTypeSpecifier(NBTType.Int);
				case "long":
					return new PrimitiveTypeSpecifier(NBTType.Long);
				case "float":
					return new PrimitiveTypeSpecifier(NBTType.Float);
				case "double":
					return new PrimitiveTypeSpecifier(NBTType.Double);
				case "string":
					return new PrimitiveTypeSpecifier(NBTType.String);
				case "list":
					return new PrimitiveTypeSpecifier(NBTType.List);
				case "nbt":
					return new PrimitiveTypeSpecifier(NBTType.Compound);
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

		protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false) => new ListTypeSpecifier(Inner.Resolve(ctx, baseNamespace));
	}

	public class AbstractReferenceTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
	{
		public readonly AbstractTypeSpecifier Inner = inner;

		protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false) => new ReferenceTypeSpecifier(Inner.Resolve(ctx, baseNamespace));
	}

	public class AbstractWeakReferenceTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
	{
		public readonly AbstractTypeSpecifier Inner = inner;

		protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false) => new WeakReferenceTypeSpecifier(Inner.Resolve(ctx, baseNamespace));
	}

	public class AbstractStructTypeSpecifier(LocationRange loc, NamespacedID id, AbstractTypeSpecifier? baseClass, Dictionary<string, AbstractTypeSpecifier> props, List<MethodNode> methods) : AbstractTypeSpecifier(loc), IRootChild
	{
		public readonly NamespacedID ID = id;
		public readonly Dictionary<string, AbstractTypeSpecifier> Properties = props;
		public readonly List<MethodNode> Methods = methods;
		public readonly AbstractTypeSpecifier? BaseClass = baseClass;

		public void Process(Compiler ctx, RootNode root)
		{
			var selfType = Resolve(ctx, ID.GetContainingFolder());
			ctx.IR.AddType(new(ID, Location, selfType, this));

			bool hasConstructor = false;

			foreach (var i in Methods)
			{
				if (i is ConstructorNode)
				{
					hasConstructor = true;
				}

				i.Process(ctx, root);
			}

			if (ctx.IR.GetConstructorOrNull(selfType.BaseClass) is not null && !hasConstructor)
			{
				throw new MissingConstructorError(selfType.BaseClass.ToString());
			}

			if (Properties.Count != 0 && selfType.EffectiveType != NBTType.Compound)
			{
				throw new PropertyError(selfType.ToString());
			}
		}

		protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false)
		{
			var baseClass = BaseClass?.Resolve(ctx, baseNamespace) ?? PrimitiveTypeSpecifier.Compound;

			return new StructTypeSpecifier(ID, baseClass, new(Properties.Select(i => new KeyValuePair<string, TypeSpecifier>(i.Key, i.Value.Resolve(ctx, baseNamespace)))));
		}
	}
}
