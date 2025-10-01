using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.Types;
using Datapack.Net.Data;

namespace Amethyst.AST
{
	public abstract class AbstractTypeSpecifier(LocationRange loc) : Node(loc)
	{
		public TypeSpecifier Resolve(FunctionContext ctx, bool allowAuto = false) => Resolve(ctx.Compiler, allowAuto);

		public TypeSpecifier Resolve(Compiler ctx, bool allowAuto = false)
		{
			TypeSpecifier? ret = null;
			if (!ctx.WrapError(Location, () => ret = _Resolve(ctx, allowAuto))) throw new EmptyAmethystError();
			return ret!;
		}

		protected abstract TypeSpecifier _Resolve(Compiler ctx, bool allowAuto = false);
	}

	public class SimpleAbstractTypeSpecifier(LocationRange loc, string type) : AbstractTypeSpecifier(loc)
	{
		public readonly string Type = type;

		protected override TypeSpecifier _Resolve(Compiler ctx, bool allowAuto = false)
		{
			switch (Type)
			{
				case "void":
					return new VoidTypeSpecifier();
				case "var":
					if (allowAuto) return new VarTypeSpecifier();
					else throw new InvalidTypeError(Type);
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
					break;
			}

			throw new UnknownTypeError(Type);
		}
	}

	public class AbstractListTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
	{
		public readonly AbstractTypeSpecifier Inner = inner;

		protected override TypeSpecifier _Resolve(Compiler ctx, bool allowAuto = false) => new ListTypeSpecifier(Inner.Resolve(ctx));
	}

	public class AbstractReferenceTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
	{
		public readonly AbstractTypeSpecifier Inner = inner;

		protected override TypeSpecifier _Resolve(Compiler ctx, bool allowAuto = false) => new ReferenceTypeSpecifier(Inner.Resolve(ctx));
	}

	public class AbstractWeakReferenceTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
	{
		public readonly AbstractTypeSpecifier Inner = inner;

		protected override TypeSpecifier _Resolve(Compiler ctx, bool allowAuto = false) => new WeakReferenceTypeSpecifier(Inner.Resolve(ctx));
	}

	public class AbstractInterfaceTypeSpecifier(LocationRange loc, string name, Dictionary<string, AbstractObjectProperty> props) : AbstractTypeSpecifier(loc), IRootChild
	{
		public readonly string Name = name;
		public readonly Dictionary<string, AbstractObjectProperty> Properties = props;

		public void Process(Compiler ctx)
		{
			throw new NotImplementedException();
		}

		protected override TypeSpecifier _Resolve(Compiler ctx, bool allowAuto = false) => new InterfaceType(new(Properties.Select(i => new KeyValuePair<string, ObjectProperty>(i.Key, new ObjectProperty(i.Value.Type.Resolve(ctx), i.Value.Name)))));
	}

	public readonly record struct AbstractObjectProperty(AbstractTypeSpecifier Type, string Name);
}
