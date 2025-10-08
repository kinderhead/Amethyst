using Amethyst.Errors;
using Amethyst.IR.Types;
using Datapack.Net.Data;
using Datapack.Net.Utils;
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
			var selfType = (StructTypeSpecifier)Resolve(ctx, ID.GetContainingFolder());
			ctx.IR.AddType(new(ID, Location, selfType));

			var hasConstructor = false;

			foreach (var i in Methods)
			{
				if (i is ConstructorNode)
				{
					hasConstructor = true;
				}
				else if (selfType.HierarchyMethod(i.ID.GetFile()) is FunctionTypeSpecifier other)
				{
					var otherIsVirtual = other.Modifiers.HasFlag(FunctionModifiers.Virtual);
					var thisIsVirtual = i.Modifiers.HasFlag(FunctionModifiers.Virtual);

					if (otherIsVirtual && !thisIsVirtual)
					{
						throw new MissingVirtualError(i.ID.GetFile());
					}
					else if (!otherIsVirtual)
					{
						throw new CannotOverrideError(i.ID.GetFile());
					}
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

			var props = new Dictionary<string, TypeSpecifier>();
			var methods = new Dictionary<string, FunctionTypeSpecifier>();

			foreach (var (k, v) in Properties)
			{
				props[k] = v.Resolve(ctx, baseNamespace);
			}

			foreach (var i in Methods)
			{
				if (i is ConstructorNode)
				{
					continue;
				}

				var type = i.GetFunctionType(ctx);
				methods[i.ID.GetFile()] = type;

				if (i.Modifiers.HasFlag(FunctionModifiers.Virtual))
				{
					props[i.ID.GetFile()] = type;
				}
			}

			return new StructTypeSpecifier(ID, baseClass, props, methods);
		}
	}
}
