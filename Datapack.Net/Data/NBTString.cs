using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public class NBTString(string val) : NBTValue
    {
		public override NBTType Type => NBTType.String;
		public readonly string Value = val;

        public override void Build(StringBuilder sb)
        {
            sb.Append($"\"{Escape(Value)}\"");
        }

        public static string Escape(string val)
        {
            val = val.Replace("\\", "\\\\");
            return val.Replace("\"", "\\\"");
        }

		public static implicit operator NBTString(string val) => new(val);
        public static implicit operator string(NBTString val) => val.Value;
    }

    public class NBTRawString(string val) : NBTValue
    {
        public override NBTType Type => NBTType.String;
        public readonly string Value = val;

        public override void Build(StringBuilder sb)
        {
            sb.Append(Value);
        }
    }
}
