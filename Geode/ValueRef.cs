using Datapack.Net.Data;
using Geode.Errors;
using Geode.IR;
using Geode.Values;
using System.Collections.Immutable;

namespace Geode
{
	public class ValueRef : IInstructionArg, IValueLike, ICloneable
	{
		public IValue? Value { get; private set; }
		public TypeSpecifier Type { get; private set; }

		// Used for debugging
		public readonly Instruction? SourceInsn = null;

		public bool ForceScoreReg = false;

		public bool IsLiteral => Value is not null && Value.IsLiteral;
		public bool NeedsScoreReg => Value is null && (Type.ShouldStoreInScore || ForceScoreReg);
		public bool NeedsStackVar => Value is null && !(Type.ShouldStoreInScore || ForceScoreReg);

		public string Name { get => field is null ? Value is not null ? $"{(Value.IsLiteral || Value is DataTargetValue ? "" : "%")}{Value}" : "" : field; set; }

		private readonly HashSet<ValueRef> dependencies = [];
		public IReadOnlySet<ValueRef> Dependencies => dependencies;

		public ValueRef(IValue val)
		{
			Value = val;
			Type = val.Type;
			dependencies = [this];
		}

		public ValueRef(TypeSpecifier type, Instruction? insn = null)
		{
			Type = type;
			dependencies = [this];
			SourceInsn = insn;
		}

		public IValue Expect(NBTType type)
		{
			if (Value is null || Value.Type.EffectiveType != type)
			{
				throw new InvalidTypeError((Value is not null ? Enum.GetName(Value.Type.EffectiveType)?.ToLower() : "<error>") ?? "<error>", Enum.GetName(type)?.ToLower() ?? "<error>");
			}

			return Value;
		}

		public T Expect<T>() where T : class, IValue
		{
			if (Value is T val)
			{
				return val;
			}

#if DEBUG
			System.Diagnostics.Debugger.Break();
#endif

			throw new InvalidTypeError(Value?.GetType().Name.ToLower() ?? "<error>", typeof(T).Name.ToLower());
		}
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

		public void AddDependency(ValueRef dep) => dependencies.Add(dep);

		public void ReplaceValue(ValueRef value, ValueRef with)
		{
			if (value == this)
			{
				throw new InvalidOperationException("Cannot replace self");
			}

			if (dependencies.Remove(value))
			{
				dependencies.Add(with);
			}
		}

		public static implicit operator ValueRef(Value val) => new(val);
	}
}
