using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.Types
{
	public class PrimitiveTypeSpecifier(NBTType type) : TypeSpecifier
	{
		public override bool Operable => NBTValue.IsOperableType(Type);
		public override bool IsList => Type == NBTType.List || Type == NBTType.IntArray || Type == NBTType.LongArray || Type == NBTType.ByteArray;
		public readonly NBTType Type = type;
		public override NBTType EffectiveType => Type;

		public override string MacroGuardStart => Type == NBTType.String ? "\"" : "";
		public override string MacroGuardEnd => Type == NBTType.String ? "\"" : "";

        //public override bool IsAssignableTo(TypeSpecifier other) => (other.IsList && Type == NBTType.List) || base.IsAssignableTo(other);
        public override TypeSpecifier? Property(string name) => Type == NBTType.Compound ? new AnyTypeSpecifier() : base.Property(name);

		protected override bool AreEqual(TypeSpecifier obj) => obj is PrimitiveTypeSpecifier p && p.Type == Type;
		public override string ToString() => Enum.GetName(Type)?.ToLower() ?? throw new InvalidOperationException();
		public override object Clone() => new PrimitiveTypeSpecifier(Type);

		public override LiteralValue DefaultValue => Type switch
		{
			NBTType.Boolean => new(false),
			NBTType.Byte => new(new NBTByte(0)),
			NBTType.Short => new(new NBTShort(0)),
			NBTType.Int => new(new NBTInt(0)),
			NBTType.Long => new(new NBTLong(0)),
			NBTType.Float => new(new NBTFloat(0)),
			NBTType.Double => new(new NBTDouble(0)),
			NBTType.List => new(new NBTList()),
			NBTType.Compound => new(new NBTCompound()),
			_ => throw new NotImplementedException()
		};

		public static PrimitiveTypeSpecifier Int => new(NBTType.Int);
		public static PrimitiveTypeSpecifier Bool => new(NBTType.Boolean);
		public static PrimitiveTypeSpecifier String => new(NBTType.String);
		public static PrimitiveTypeSpecifier Compound => new(NBTType.Compound);
		public static PrimitiveTypeSpecifier List => new(NBTType.List);
	}
}
