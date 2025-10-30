using Datapack.Net.Data;
using Geode.Errors;
using Geode.Types;
using Geode.Values;
using System.Text;

namespace Geode.IR
{
	public abstract class Instruction
	{
		public IInstructionArg[] Arguments { get; private set; }
		public readonly ValueRef ReturnValue;

		public LocationRange Location = LocationRange.None;

		public abstract string Name { get; }
		public abstract NBTType?[] ArgTypes { get; }
		public abstract TypeSpecifier ReturnType { get; }

		public bool MarkedForRemoval { get; private set; } = false;
		public Instruction[] ToReplaceWith { get; private set; } = [];

		public virtual bool IsReturn => false;
		public virtual bool AlwaysUseScore => false; // Tee hee I love band aid fixes for band aid fixes

		public Instruction(IEnumerable<IInstructionArg> args)
		{
			Arguments = [.. args];
			ReturnValue = new(ReturnType);
			CheckArguments();
		}

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

		protected void Remove() => MarkedForRemoval = true;

		public void ReplaceWith(params Instruction[] insns) => ToReplaceWith = insns;

		/// <summary>
		/// Compute return value. Return null to allow Geode to allocate automatically.
		/// </summary>
		/// <returns>Value or null</returns>
		protected abstract IValue? ComputeReturnValue(FunctionContext ctx);
	}
}
