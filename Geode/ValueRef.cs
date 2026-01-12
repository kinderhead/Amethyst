using Datapack.Net.Data;
using Geode.Errors;
using Geode.IR;
using Geode.Values;

namespace Geode
{
	public class ValueRef : IInstructionArg, IValueLike, ICloneable
	{
		public IValue? Value { get; private set; }
		public TypeSpecifier Type { get; private set; }

		public bool ForceScoreReg = false;

		public bool IsLiteral => Value is not null && Value.IsLiteral;
		public bool NeedsScoreReg => Value is null && (Type.ShouldStoreInScore || ForceScoreReg);
		public bool NeedsStackVar => Value is null && !(Type.ShouldStoreInScore || ForceScoreReg);

		public string Name { get => field is null ? Value is not null ? $"{(Value.IsLiteral || Value is DataTargetValue ? "" : "%")}{Value}" : "" : field; set; }

		public HashSet<ValueRef> Dependencies { get; }

		public ValueRef(IValue val)
		{
			Value = val;
			Type = val.Type;
			Dependencies = [this];
		}

		public ValueRef(TypeSpecifier type)
		{
			Type = type;
			Dependencies = [this];
		}

		public IValue Expect(NBTType type)
		{
			if (Value is null || Value.Type.EffectiveType != type)
			{
				throw new InvalidTypeError((Value is not null ? Enum.GetName(Value.Type.EffectiveType)?.ToLower() : "<error>") ?? "<error>", Enum.GetName(type)?.ToLower() ?? "<error>");
			}

			return Value;
		}

		public T Expect<T>() where T : class, IValue => Value as T ?? throw new InvalidTypeError(Value?.GetType().Name.ToLower() ?? "<error>", typeof(T).Name.ToLower());
		public IValue Expect() => Expect<IValue>();

		public void SetValue(IValue val)
		{
			Value = val;
			Type = val.Type;
		}

		public ValueRef SetType(TypeSpecifier type)
		{
			Type = type;
			return this;
		}

		public ValueRef Clone() => Value is null ? new(Type) : new(Value);
		object ICloneable.Clone() => Clone();

		public void ReplaceValue(ValueRef value, ValueRef with)
		{
			if (value == this)
			{
				throw new InvalidOperationException("Cannot replace self");
			}
		}

		public static implicit operator ValueRef(Value val) => new(val);
	}
}
