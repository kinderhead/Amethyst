using System.Text;

namespace Datapack.Net.Data
{
	public class NBTString(string val) : NBTValue
	{
		public override NBTType Type => NBTType.String;
		public readonly string Value = val;

		public override void Build(StringBuilder sb) => sb.Append($"\"{Escape(Value)}\"");

		public static string Escape(string val)
		{
			val = val.Replace("\\", "\\\\");
			return val.Replace("\"", "\\\"");
		}

		public static string Unescape(string val)
		{
			val = val.Replace("\\\"", "\"");
			return val.Replace("\\\\", "\\");
		}

		public override NBTValue Cast(NBTNumberType type) => throw new InvalidOperationException();

		public static implicit operator NBTString(string val) => new(val);
		public static implicit operator string(NBTString val) => val.Value;
	}

	public class NBTRawString(string val) : NBTString(val)
	{
		public override void Build(StringBuilder sb) => sb.Append(Value);
	}
}
