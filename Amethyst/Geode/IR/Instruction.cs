using System.Text;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR
{
    public abstract class Instruction
    {
        public readonly IInstructionArg[] Arguments;
        public readonly ValueRef ReturnValue;

        public abstract string Name { get; }
        public abstract NBTType?[] ArgTypes { get; }
        public abstract TypeSpecifier ReturnType { get; }

        public bool MarkedForRemoval { get; private set; } = false;

        public Instruction(IEnumerable<IInstructionArg> args)
        {
            Arguments = [.. args];
            ReturnValue = new(ReturnType);

            if (Arguments.Length != ArgTypes.Length) throw new InvalidOperationException("Mismatched argument count");

            int i = 0;
            foreach (var arg in Arguments)
            {
                if (ArgTypes[i] is not null && arg is ValueRef v && ArgTypes[i] != v.Type.EffectiveType)
                {
                    throw new InvalidOperationException($"Argument {i + 1} is of type {v.Type}, but expected {ArgTypes[i]}");
                }

                i++;
            }
        }

        public T Arg<T>(int index) where T : IInstructionArg => (T)Arguments[index];

        public void Resolve()
        {
            var ret = ComputeReturnValue();

            if (ret is null) return;

            if (ret.Type != ReturnType) throw new InvalidOperationException($"Instruction returned {ret.Type}, but expected {ReturnType}");

            ReturnValue.SetValue(ret);
            if (ret.IsLiteral) Remove();
        }

        public virtual void CheckPotentialScoreReuse(Func<ValueRef, ValueRef, bool> tryLink)
        {

        }

        public virtual string Dump(Func<IInstructionArg, string> valueMap)
        {
            var builder = new StringBuilder();

            if (ReturnType is not VoidTypeSpecifier)
            {
                builder.Append($"{valueMap(ReturnValue)} = ");
            }

            builder.Append($"{Name} ");

            foreach (var i in Arguments)
            {
                builder.Append($"{valueMap(i)}, ");
            }

            if (Arguments.Length > 0) builder.Length -= 2;

            return builder.ToString();
        }

        protected bool AreArgsLiteral(out LiteralValue[] args)
        {
#pragma warning disable CS8601
            args = [.. Arguments.Select(a => (a as ValueRef)?.Value as LiteralValue)];
#pragma warning restore CS8601

            return args.All(v => v is not null);
        }

        protected bool AreArgsLiteral<T1, T2>(out T1 arg1, out T2 arg2) where T1 : NBTValue where T2 : NBTValue
        {
            if (Arguments.Length != 2) throw new InvalidOperationException("Wrong AreArgsLiteral overload");

            if (!AreArgsLiteral(out var args))
            {
                arg1 = null!;
                arg2 = null!;
                return false;
            }

            arg1 = args[0].Value as T1 ?? throw new InvalidCastException($"Expected {typeof(T1)}");
            arg2 = args[1].Value as T2 ?? throw new InvalidCastException($"Expected {typeof(T2)}");

            return true;
        }

        protected void Remove() => MarkedForRemoval = true;

        /// <summary>
        /// Compute return value. Return null to allow Geode to allocate automatically.
        /// </summary>
        /// <returns>Value or null</returns>
        protected abstract Value? ComputeReturnValue();
    }
}
