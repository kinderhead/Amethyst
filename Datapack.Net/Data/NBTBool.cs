using System.Text;

namespace Datapack.Net.Data
{
	public class NBTBool(bool val) : NBTValue
	{
		public override NBTType Type => NBTType.Boolean;
		public readonly bool Value = val;

		public override void Build(StringBuilder sb) => sb.Append(Value ? "true" : "false");

		public override NBTValue Cast(NBTNumberType type) => type switch
		{
			NBTNumberType.Boolean => this,
			NBTNumberType.Byte => (NBTValue)Convert.ToSByte(Value),
			NBTNumberType.Short => (NBTValue)Convert.ToInt16(Value),
			NBTNumberType.Int => (NBTValue)Convert.ToInt32(Value),
			NBTNumberType.Long => (NBTValue)Convert.ToInt64(Value),
			NBTNumberType.Float => (NBTValue)Convert.ToSingle(Value),
			NBTNumberType.Double => (NBTValue)Convert.ToDouble(Value),
			_ => throw new NotImplementedException(),
		};

		public static implicit operator NBTBool(bool val) => new(val);
		public static implicit operator bool(NBTBool val) => val.Value;
	}
}
