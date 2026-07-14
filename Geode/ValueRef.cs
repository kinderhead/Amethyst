using Geode.IR;
using Geode.Values;

namespace Geode
{
    public class ValueRef : IInstructionArg, IValueLike, ICloneable
    {
        private static int valueCounter;

        private readonly HashSet<ValueRef> dependencies;

        public readonly int ID = valueCounter++;

        // Used for debugging
        public readonly Instruction? SourceInsn;

        public bool ForceScoreReg = false;
        private TypeSpecifier type;

        public ValueRef(IValue val)
        {
            Value = val;
            type = val.Type;
            dependencies = [this];
        }

        public ValueRef(TypeSpecifier type, Instruction? insn = null)
        {
            this.type = type;
            dependencies = [this];
            SourceInsn = insn;
        }

        public bool IsLiteral => Value is not null && Value.IsLiteral;
        public bool NeedsScoreReg => Value is null && (Type.ShouldStoreInScore || ForceScoreReg);
        public bool NeedsStackVar => Value is null && !(Type.ShouldStoreInScore || ForceScoreReg);
        object ICloneable.Clone() => Clone();

        public string Name
        {
            get => field is null
                ? Value is not null ? $"{(Value.IsLiteral || Value is DataTargetValue ? "" : "%")}{Value}" : ""
                : field;
            set;
        }

        public IReadOnlySet<ValueRef> Dependencies => dependencies;

        public void ReplaceValue(ValueRef value, ValueRef with)
        {
            if (value == this) throw new InvalidOperationException("Cannot replace self");

            if (dependencies.Remove(value)) dependencies.Add(with);
        }

        public IValue? Value { get; private set; }

        public TypeSpecifier Type
        {
            get => type;
            set => SetType(value);
        }

        public ValueRef ToValueRef() => this;

        public void SetValue(IValue val)
        {
            Value = val;
            Type = val.Type;
        }

        public ValueRef SetType(TypeSpecifier type)
        {
            this.type = type;
            //Value?.Type = type;
            return this;
        }

        public ValueRef Clone() => Value is null ? new(Type) : new(Value);

        public void AddDependency(ValueRef dep)
        {
            if (dependencies.Add(dep))
            {
                foreach (var i in dep.Dependencies)
                {
                    AddDependency(i);
                }
            }
        }

        public override string ToString()
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!string.IsNullOrEmpty(Name))
            {
#if DEBUG
                return $"#{ID}<{Name}>";
#else
				return Name;
#endif
            }

            return $"#{ID}";
        }


        public static implicit operator ValueRef(Value val) => new(val);
    }
}