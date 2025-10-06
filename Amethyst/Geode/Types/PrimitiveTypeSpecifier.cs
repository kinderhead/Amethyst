using Amethyst.Geode.Values;
using Datapack.Net.Data;
using Datapack.Net.Utils;

namespace Amethyst.Geode.Types
{
	public class PrimitiveTypeSpecifier(NBTType type) : TypeSpecifier
	{
		public override bool IsList => Type is NBTType.List or NBTType.IntArray or NBTType.LongArray or NBTType.ByteArray;
		public readonly NBTType Type = type;
		public override NBTType EffectiveType => Type;
		public override NamespacedID ID => $"minecraft:{this}";

		public override TypeSpecifier? DefaultPropertyType => Type == NBTType.Compound ? Compound : null;

		public override LiteralValue DefaultValue => Type switch
		{
			NBTType.Boolean => new(false),
			NBTType.Byte => new(new NBTByte(0)),
			NBTType.Short => new(new NBTShort(0)),
			NBTType.Int => new(new NBTInt(0)),
			NBTType.Long => new(new NBTLong(0)),
			NBTType.Float => new(new NBTFloat(0)),
			NBTType.Double => new(new NBTDouble(0)),
			NBTType.String => new(new NBTString("")),
			NBTType.List => new(new NBTList()),
			NBTType.Compound => new(new NBTCompound()),
			_ => throw new NotImplementedException()
		};

		public override TypeSpecifier BaseClass => Type switch
		{
			NBTType.Boolean => Byte,
			NBTType.Byte => Int,
			NBTType.Short => Int,
			_ => Compound
		};


		protected override bool EqualsImpl(TypeSpecifier obj) => obj is PrimitiveTypeSpecifier p && p.Type == Type;
		public override string ToString() => Type == NBTType.Compound ? "nbt" : Enum.GetName(Type)?.ToLower() ?? throw new InvalidOperationException();
		public override object Clone() => new PrimitiveTypeSpecifier(Type);

		public static PrimitiveTypeSpecifier Int => new(NBTType.Int);
		public static PrimitiveTypeSpecifier Bool => new(NBTType.Boolean);
		public static PrimitiveTypeSpecifier Byte => new(NBTType.Byte);
		public static PrimitiveTypeSpecifier String => new(NBTType.String);
		public static PrimitiveTypeSpecifier Compound => new(NBTType.Compound);
		public static PrimitiveTypeSpecifier List => new(NBTType.List);
	}
}
