using Amethyst.Geode.IR;
using Datapack.Net.Data;

namespace Amethyst.Geode
{
    public class ValueRef : IInstructionArg
    {
        public Value? Value { get; private set; }
        public TypeSpecifier Type { get; private set; }

        public bool IsLiteral => Value is not null && Value.IsLiteral;
        public bool NeedsScoreReg => Value is null && Type.EffectiveType == NBTType.Int;

        private string? customName;
        public string Name => customName is null ? Value is not null ? $"{(Value.IsLiteral ? "" : "%")}{Value}" : "" : customName;

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

        public void SetCustomName(string? name) => customName = name;

        public static implicit operator ValueRef(Value val) => new(val);
    }
}
