using Amethyst.AST;
using Amethyst.Errors;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;
using System.Text;

namespace Amethyst.Geode.IR
{
	public abstract class Instruction
	{
		public readonly IInstructionArg[] Arguments;
		public readonly ValueRef ReturnValue;

		public LocationRange Location = LocationRange.None;

		public abstract string Name { get; }
		public abstract NBTType?[] ArgTypes { get; }
		public abstract TypeSpecifier ReturnType { get; }

		public bool MarkedForRemoval { get; private set; } = false;

		public virtual bool IsReturn => false;
		public virtual bool ShouldProcessArgs => true;

		public Instruction(IEnumerable<IInstructionArg> args)
		{
			Arguments = [.. args];
			ReturnValue = new(ReturnType);
		}

		public void ProcessArgs(FunctionContext ctx)
		{
			if (ShouldProcessArgs)
			{
				for (var i = 0; i < Arguments.Length; i++)
				{
					if (Arguments[i] is ValueRef arg)
					{
						Arguments[i] = arg.Type.ProcessArg(arg, ctx);
					}
				}
			}

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
			var ret = ComputeReturnValue(ctx);

			if (ret is null)
			{
				if (ReturnValue.NeedsStackVar)
				{
					ReturnValue.SetValue(ctx.Temp(ReturnType));
				}

				return;
			}

			if (ReturnType is not AnyTypeSpecifier && ret.Type != ReturnType)
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

			if (ReturnType is not VoidTypeSpecifier)
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

		protected bool AreArgsLiteral(out LiteralValue[] args)
		{
#pragma warning disable CS8601
			args = [.. Arguments.Select(a => (a as ValueRef)?.Value as LiteralValue)];
#pragma warning restore CS8601

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

		/// <summary>
		/// Compute return value. Return null to allow Geode to allocate automatically.
		/// </summary>
		/// <returns>Value or null</returns>
		protected abstract Value? ComputeReturnValue(FunctionContext ctx);
	}
}
