using Datapack.Net.Data;
using Geode.Errors;
using Geode.IR;
using Geode.Values;

namespace Geode
{
	public class ValueRef : IInstructionArg
	{
		public Value? Value { get; private set; }
		public TypeSpecifier Type { get; private set; }

		public bool IsLiteral => Value is not null && Value.IsLiteral;
		public bool NeedsScoreReg => Value is null && Type.ShouldStoreInScore;
		public bool NeedsStackVar => Value is null && !Type.ShouldStoreInScore;

		public string Name { get => field is null ? Value is not null ? $"{(Value.IsLiteral || Value is DataTargetValue ? "" : "%")}{Value}" : "" : field; set; }

		public HashSet<ValueRef> Dependencies { get; } = [];

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
				throw new InvalidTypeError((Value is not null ? Enum.GetName(Value.Type.EffectiveType)?.ToLower() : "<error>") ?? "<error>", Enum.GetName(type)?.ToLower() ?? "<error>");
			}

			return Value;
		}

		public T Expect<T>() where T : Value => Value as T ?? throw new InvalidTypeError(Value?.GetType().Name.ToLower() ?? "<error>", typeof(T).Name.ToLower());
		public Value Expect() => Expect<Value>();

		public void SetValue(Value val)
		{
			Value = val;
			Type = val.Type;
		}

		public ValueRef SetType(TypeSpecifier type)
		{
			Type = type;
			return this;
		}

		public static implicit operator ValueRef(Value val) => new(val);
	}
}
