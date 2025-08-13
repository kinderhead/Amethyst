using Datapack.Net.Data;

namespace Amethyst.Geode
{
    public class ValueRef
    {
        public Value? Value { get; private set; }
        public TypeSpecifier Type { get; private set; }

        public bool IsLiteral => Value is not null && Value.IsLiteral;

        public ValueRef(Value val)
        {
            Value = val;
            Type = val.Type;
        }

        public ValueRef(TypeSpecifier type)
        {
            Type = type;
        }

        public Value Expect(NBTType type)
        {
            if (Value is null || Value.Type.EffectiveType != type)
            {
                throw new InvalidCastException($"Expected value of type \"{Enum.GetName(type)}\", but got \"{(Value is not null ? Enum.GetName(Value.Type.EffectiveType) : "null")}\"");
            }

            return Value;
        }

        public void SetValue(Value val)
        {
            Value = val;
            Type = val.Type;
        }

        public static implicit operator ValueRef(Value val) => new(val);
    }
}
