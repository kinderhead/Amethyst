using System.Text;
using Datapack.Net.Data;
using Geode.Errors;
using Geode.Types;
using Geode.Values;

namespace Geode.IR
{
	public interface IBasicInstruction
	{
		public IInstructionArg[] Arguments { get; }
		public ValueRef ReturnValue { get; }
		public string Name { get; }
		public TypeSpecifier ReturnType { get; }

		public void Remove();
	}

	public abstract class Instruction : IBasicInstruction
	{
		public virtual IInstructionArg[] Arguments { get; private set; }
		public virtual IEnumerable<ValueRef> Dependencies => Arguments.SelectMany(i => i.Dependencies);
		public ValueRef ReturnValue { get; private init; }

		public LocationRange Location = LocationRange.None;

		public abstract string Name { get; }
		public abstract NBTType?[] ArgTypes { get; }
		public abstract TypeSpecifier ReturnType { get; }

		public bool MarkedForRemoval { get; private set; } = false;
		public Instruction[] ToReplaceWith { get; private set; } = [];

		public virtual bool IsReturn => false;
		public virtual bool AlwaysUseScore => false; // Tee hee I love band aid fixes for band aid fixes (but it's actually clean!)
		public virtual bool ArgumentsAliveAtInsn => true;

		/// <summary>
		/// Affects values outside of the return value
		/// </summary>
		public virtual bool HasSideEffects { get; }

		public Instruction(IEnumerable<IInstructionArg> args)
		{
			Arguments = [.. args];
			ReturnValue = new(ReturnType, this);
			CheckArguments();
		}

		public virtual void OnAdd(Block block) { }

		public virtual void CheckArguments()
		{
			if (Arguments.Length != ArgTypes.Length)
			{
				throw new MismatchedArgumentCountError(ArgTypes.Length, Arguments.Length);
			}

			var i = 0;
			foreach (var arg in Arguments)
			{
				// Merge if statements

				if (ArgTypes[i] is not null && arg is ValueRef v && ArgTypes[i] != v.Type.EffectiveType)
				{
					if (!(ArgTypes[i] == NBTType.Int && (v.NeedsScoreReg || v.Value is ScoreValue)))
					{
						throw new InvalidTypeError(v.Type.ToString(), $"{ArgTypes[i]}".ToLower()); // Use string interpolation to handle the null case
					}
				}

				i++;
			}
		}

		public T Arg<T>(int index) where T : IInstructionArg => (T)Arguments[index];

		public void Resolve(FunctionContext ctx)
		{
			ReturnValue.ForceScoreReg = AlwaysUseScore;
			var ret = ComputeReturnValue(ctx);

			if (ret is null)
			{
				if (ReturnValue.NeedsStackVar)
				{
					ReturnValue.SetValue(ctx.Temp(ReturnType));
				}

				return;
			}

			if (ReturnType is not AnyType && ret.Type != ReturnType)
			{
				throw new InvalidTypeError(ret.Type.ToString(), ReturnType.ToString());
			}

			ReturnValue.SetValue(ret);
			if (ret.IsLiteral)
			{
				Remove();
			}
		}

		public abstract void Render(RenderContext ctx);
		public virtual void ConfigureLifetime(Func<ValueRef, ValueRef, bool> tryLink, Action<ValueRef, ValueRef> markOverlap) { }

		public virtual string Dump(Func<IInstructionArg, string> valueMap)
		{
			var builder = new StringBuilder();

			if (ReturnType is not VoidType)
			{
				builder.Append($"{valueMap(ReturnValue)} = ");
			}

			builder.Append($"{Name} ");

			foreach (var i in Arguments)
			{
				builder.Append($"{valueMap(i)}, ");
			}

			if (Arguments.Length > 0)
			{
				builder.Length -= 2;
			}

			return builder.ToString();
		}

		public virtual Instruction Clone()
		{
			var insn = (Instruction)MemberwiseClone();
			insn.Arguments = [.. Arguments];
			return insn;
		}

		public virtual void ReplaceValue(ValueRef val, ValueRef with)
		{
			// There's got to be a better way to do this
			for (int i = 0; i < Arguments.Length; i++)
			{
				if (Arguments[i] == val)
				{
					Arguments[i] = with;
				}
				else
				{
					Arguments[i].ReplaceValue(val, with);
				}
			}
		}

		public virtual bool ContainsStoreFor(Variable variable) => false;

		protected bool AreArgsLiteral(out LiteralValue[] args)
		{
			args = [.. Arguments.Select(a => ((a as ValueRef)?.Value as LiteralValue)!)];

			return args.All(v => v is not null);
		}

		protected bool AreArgsLiteral<T1, T2>(out T1 arg1, out T2 arg2) where T1 : NBTValue where T2 : NBTValue
		{
			if (Arguments.Length != 2)
			{
				throw new InvalidOperationException("Wrong AreArgsLiteral overload");
			}

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

		public virtual void Remove() => MarkedForRemoval = true;

		public void ReplaceWith(params Instruction[] insns) => ToReplaceWith = insns;

		/// <summary>
		/// Compute return value. Return null to allow Geode to allocate automatically.
		/// </summary>
		/// <returns>Value or null</returns>
		protected abstract IValue? ComputeReturnValue(FunctionContext ctx);
	}

	public abstract class DynamicInstruction() : Instruction([])
	{
		public sealed override NBTType?[] ArgTypes => [];
		public override IInstructionArg[] Arguments => throw new InvalidOperationException($"{Name} handles arguments manually");
		public override abstract IEnumerable<ValueRef> Dependencies { get; }

		public override void CheckArguments() { }

		public abstract override void ReplaceValue(ValueRef val, ValueRef with);
	}
}
