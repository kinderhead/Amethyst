using System.Text;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.IR.Instructions;
using Geode.IR.Passes;
using Geode.Util;
using Geode.Values;

namespace Geode.IR
{
	public class Block(string name, NamespacedID funcID, FunctionContext ctx) : GraphNode<Block>, IInstructionArg
	{
		public string Name => name;
		public readonly FunctionContext Ctx = ctx;
		public readonly PhiContext Phi = new();

		private readonly List<Instruction> instructions = [];
		public IReadOnlyList<Instruction> Instructions => instructions;
		public IEnumerable<PhiInsn> PhiInsns => Instructions.Where(i => i is PhiInsn).Cast<PhiInsn>();

		public readonly MCFunction Function = new(funcID, true);

		public bool ForkGuard { get; private set; }

		public IReadOnlySet<ValueRef> Dependencies { get; } = new HashSet<ValueRef>();

		public ValueRef Prepend(Instruction insn, string? customName = null)
		{
			instructions.Insert(0, insn);
			return ProcessInsn(insn, customName);
		}

		public ValueRef Add(Instruction insn, string? customName = null)
		{
			if (instructions.Count == 0 || instructions.Last() is not IBlockCapstoneInsn)
			{
				instructions.Add(insn);
				insn.OnAdd(this);
			}

			return ProcessInsn(insn, customName);
		}

		private ValueRef ProcessInsn(Instruction insn, string? customName)
		{
			if (Ctx.LocationStack.Count != 0)
			{
				insn.Location = Ctx.LocationStack.Peek();
			}

			insn.ReturnValue.Name = customName!;

			return insn.ReturnValue;
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
			if (ForkGuard)
			{
				var returning = ctx.GetIsFunctionReturningValue();
				Function.Add(new Execute().If.Data(returning.Storage, returning.Path).Run(new ReturnCommand(0)));
			}

			var renderer = GetRenderCtx(builder, ctx);
			
			foreach (var i in Instructions)
			{
				if (!ctx.Compiler.WrapError(i.Location, ctx, () =>
				{
					i.Render(renderer);
				}))
				{
					throw new EmptyGeodeError();
				}
			}

			builder.Register(Function);
		}

		public void Cleanse()
		{
			instructions.RemoveAll(x => x.MarkedForRemoval);

			for (var i = 0; i < instructions.Count; i++)
			{
				if (instructions[i].ToReplaceWith.Length != 0)
				{
					instructions.InsertRange(i + 1, instructions[i].ToReplaceWith);
					instructions.RemoveAt(i);
					i--;
				}
			}
		}

		public (List<Instruction> insns, List<Variable> variables) Copy(string newVariableBaseLoc)
		{
			List<Instruction> insns = [];
			List<Variable> variables = [];
			Dictionary <ValueRef, ValueRef> valueMap = [];

			ValueRef map(ValueRef val)
			{
				if (valueMap.TryGetValue(val, out var ret))
				{
					return ret;
				}

				var newValue = val.Clone();

				if (newValue.Value is Variable v)
				{
					var newVariable = new Variable(v.Name, Ctx.Compiler.IR.RuntimeID, newVariableBaseLoc, v.Frame, v.Type);
					newValue.SetValue(newVariable);
					variables.Add(newVariable);
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

			return (insns, variables);
		}

		public bool ContainsStoreFor(Variable variable) => Instructions.Any(i => i.ContainsStoreFor(variable));

		public void EnableForkGuard() => ForkGuard = true;

		public override string ToString() => Name;

		public RenderContext GetRenderCtx(GeodeBuilder builder, FunctionContext ctx) => new(Function, this, builder, ctx);
		public void ReplaceValue(ValueRef value, ValueRef with) { }
	}
}
