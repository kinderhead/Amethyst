using Datapack.Net.Function;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.IR.Instructions;
using Geode.Values;
using System.Text;

namespace Geode.IR
{
	public class Block(string name, NamespacedID funcID, FunctionContext ctx) : IInstructionArg
	{
		public string Name => name;
		public readonly FunctionContext Ctx = ctx;

		public readonly List<Instruction> Instructions = [];
		public readonly List<Block> Previous = [];
		public readonly List<Block> Next = [];

		public readonly MCFunction Function = new(funcID, true);

		public HashSet<ValueRef> Dependencies { get; } = [];

		public ValueRef Add(Instruction insn)
		{
			if (Instructions.Count == 0 || Instructions.Last() is not ReturnInsn)
			{
				Instructions.Add(insn);
			}

			return insn.ReturnValue;
		}

		public void InsertAtBeginning(params IEnumerable<Instruction> insns) => Instructions.InsertRange(0, insns);

		public void Link(Block next)
		{
			Next.Add(next);
			next.Previous.Add(this);
		}

		public string Dump(Func<IInstructionArg, string> valueMap)
		{
			var builder = new StringBuilder();

			builder.AppendLine($"{Name}:");
			foreach (var i in Instructions)
			{
				builder.AppendLine($"    {i.Dump(valueMap)}");
			}

			return builder.ToString();
		}

		public void Render(GeodeBuilder builder, FunctionContext ctx)
		{
			foreach (var i in Instructions)
			{
				if (!ctx.Compiler.WrapError(i.Location, ctx, () =>
				{
					i.Render(GetRenderCtx(builder, ctx));
				}))
				{
					throw new EmptyGeodeError();
				}
			}

			builder.Register(Function);
		}

		public List<Instruction> Copy(string newVariableBaseLoc)
		{
			List<Instruction> insns = [];
			Dictionary<ValueRef, ValueRef> valueMap = [];

			ValueRef map(ValueRef val)
			{
				if (valueMap.TryGetValue(val, out var ret))
				{
					return ret;
				}

				var newValue = val.Clone();

				if (newValue.Value is Variable v)
				{
					newValue.SetValue(new Variable(v.Name, Ctx.Compiler.IR.RuntimeID, newVariableBaseLoc, v.Frame, v.Type));
				}

				valueMap[val] = newValue;
				return newValue;
			}

			foreach (var i in Instructions)
			{
				var newInsn = i.Clone();

				for (var j = 0; j < i.Arguments.Length; j++)
				{
					if (i.Arguments[j] is not ValueRef val)
					{
						throw new NotImplementedException("This block cannot be copied. Try not inlining this function.");
					}

					newInsn.Arguments[j] = map(val);
					valueMap[i.ReturnValue] = newInsn.ReturnValue;
				}

				insns.Add(newInsn);
			}

			return insns;
		}

		public RenderContext GetRenderCtx(GeodeBuilder builder, FunctionContext ctx) => new(Function, this, builder, ctx);
	}
}
