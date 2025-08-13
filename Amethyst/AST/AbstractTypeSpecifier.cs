using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Datapack.Net.Data;

namespace Amethyst.AST
{
	public abstract class AbstractTypeSpecifier(LocationRange loc) : Node(loc)
	{
		public TypeSpecifier Resolve(FunctionContext ctx, bool allowAuto = false) => Resolve(ctx.Compiler, allowAuto);
		public abstract TypeSpecifier Resolve(Compiler ctx, bool allowAuto = false);
	}

	public class SimpleAbstractTypeSpecifier(LocationRange loc, string type) : AbstractTypeSpecifier(loc)
	{
		public readonly string Type = type;

		public override TypeSpecifier Resolve(Compiler ctx, bool allowAuto = false)
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

		public override TypeSpecifier Resolve(Compiler ctx, bool allowAuto = false) => new ListTypeSpecifier(Inner.Resolve(ctx, allowAuto));
	}

	public class AbstractInterfaceTypeSpecifier(LocationRange loc, string name, Dictionary<string, AbstractObjectProperty> props) : AbstractTypeSpecifier(loc), IRootChild
	{
		public readonly string Name = name;
		public readonly Dictionary<string, AbstractObjectProperty> Properties = props;

		public void Process(Compiler ctx)
		{
			throw new NotImplementedException();
		}

		public override TypeSpecifier Resolve(Compiler ctx, bool allowAuto = false) => new InterfaceType(new(Properties.Select(i => new KeyValuePair<string, ObjectProperty>(i.Key, new ObjectProperty(i.Value.Type.Resolve(ctx), i.Value.Name)))));
	}

	public readonly record struct AbstractObjectProperty(AbstractTypeSpecifier Type, string Name);
}
