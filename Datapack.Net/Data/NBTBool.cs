using System.Text;

namespace Datapack.Net.Data
{
	public class NBTBool(bool val) : NBTValue
	{
		public readonly bool Value = val;
		public override NBTType Type => NBTType.Boolean;

		public override void Build(StringBuilder sb) => sb.Append(Value ? "true" : "false");

		public override NBTValue Cast(NBTNumberType type) => type switch
		{
			NBTNumberType.Boolean => this,
			NBTNumberType.Byte => Convert.ToSByte(Value),
			NBTNumberType.Short => Convert.ToInt16(Value),
			NBTNumberType.Int => Convert.ToInt32(Value),
			NBTNumberType.Long => Convert.ToInt64(Value),
			NBTNumberType.Float => Convert.ToSingle(Value),
			NBTNumberType.Double => Convert.ToDouble(Value),
			_ => throw new NotImplementedException()
		};

		public static implicit operator NBTBool(bool val) => new(val);
		public static implicit operator bool(NBTBool val) => val.Value;
		public static implicit operator NBTInt(NBTBool val) => new(val.Value ? 1 : 0);
	}
}