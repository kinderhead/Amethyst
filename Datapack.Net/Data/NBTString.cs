using System.Text;

namespace Datapack.Net.Data
{
    public class NBTString(string val) : NBTValue
    {
        public readonly string Value = val;
        public override NBTType Type => NBTType.String;

        public override void Build(StringBuilder sb) => sb.Append($"\"{Escape(Value)}\"");

        public static string Escape(string val)
        {
            var builder = new StringBuilder();

            foreach (var i in val)
            {
                switch (i)
                {
                    case '\b':
                        builder.Append(@"\b");
                        break;
                    case '\f':
                        builder.Append(@"\f");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '\\':
                        builder.Append(@"\\");
                        break;
                    case '\'':
                        builder.Append(@"\'");
                        break;
                    case '"':
                        builder.Append(@"\""");
                        break;
                    default:
                        builder.Append(i);
                        break;
                }
            }

            return builder.ToString();
        }

        public static string Unescape(string val)
        {
            var builder = new StringBuilder();
            var str = new Queue<char>(val);

            while (str.Count != 0)
            {
                var c = str.Dequeue();
                if (c == '\\')
                {
                    switch (str.Dequeue())
                    {
                        case 'b':
                            builder.Append('\b');
                            break;
                        case 'f':
                            builder.Append('\f');
                            break;
                        case 'n':
                            builder.Append('\n');
                            break;
                        case 'r':
                            builder.Append('\r');
                            break;
                        case 's':
                            builder.Append(' ');
                            break;
                        case 't':
                            builder.Append('\t');
                            break;
                        case '\\':
                            builder.Append('\\');
                            break;
                        case '\'':
                            builder.Append('\'');
                            break;
                        case '"':
                            builder.Append('"');
                            break;
                    }
                }
                else builder.Append(c);
            }

            return builder.ToString();
        }

        public override NBTValue Cast(NBTNumberType type) => throw new InvalidOperationException("Cannot cast string to number");
        protected override bool Equals(NBTValue other) => other is NBTString str && str.Value == Value;
        public override int GetHashCode() => HashCode.Combine(Value, Type);

        public static implicit operator NBTString(string val) => new(val);
        public static implicit operator string(NBTString val) => val.Value;
    }

    public class NBTRawString(string val) : NBTString(val)
    {
        public override void Build(StringBuilder sb) => sb.Append(Value);
        protected override bool Equals(NBTValue other) => other is NBTRawString str && str.Value == Value;
        public override int GetHashCode() => HashCode.Combine(Value, Type, "NBTRawString");
    }
}