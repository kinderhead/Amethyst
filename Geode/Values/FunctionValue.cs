using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.IR;
using Geode.Types;

namespace Geode.Values
{
	public class FunctionValue(NamespacedID id, FunctionTypeSpecifier type) : LiteralValue(new NBTString(id.ToString())), IFunctionLike
	{
		public readonly NamespacedID ID = id;
		public override TypeSpecifier Type => type;
		public override string ToString() => ID.ToString();
		public FunctionTypeSpecifier FuncType => (FunctionTypeSpecifier)Type;

		public virtual void Call(RenderContext ctx, ValueRef[] args)
		{
			var processedMacros = SetArgsAndGetMacros(ctx, FuncType, args);

			if (processedMacros is StorageValue s)
			{
				ctx.PossibleErrorChecker(new FunctionCommand(ID, s.Storage, s.Path),
					text => text
						.Text($": Failed to run function ")
						.Text(ID.ToString(), new FormattedText.Modifiers { Color = "red", SuggestCommand = $"/function {ID}", Underlined = true })
						.Text($" with macro arguments: "),
					s
				);
			}
			else if (processedMacros is LiteralValue l)
			{
				ctx.PossibleErrorChecker(new FunctionCommand(ID, (NBTCompound)l.Value),
					text => text
						.Text($": Failed to run function ")
						.Text(ID.ToString(), new FormattedText.Modifiers { Color = "red", SuggestCommand = $"/function {ID} {l.Value}", Underlined = true })
						.Text($" with macro arguments: "),
					l
				);
			}
			else
			{
				ctx.PossibleErrorChecker(new FunctionCommand(ID), $"Failed to run function \"{ID}\"");
			}
		}

		public virtual IFunctionLike CloneWithType(FunctionTypeSpecifier type) => new FunctionValue(ID, type);

		public ValueRef AsMethod(ValueRef self, FunctionContext ctx) => new MethodValue(this, ctx.ImplicitCast(self, FuncType.Parameters[0].Type));

		public static Value? SetArgsAndGetMacros(RenderContext ctx, FunctionTypeSpecifier funcType, ValueRef[] args)
		{
			Value? processedMacros = null;

			if (args.Length != funcType.Parameters.Length)
			{
				throw new MismatchedArgumentCountError(funcType.Parameters.Length, args.Length);
			}

			if (args.Length != 0)
			{
				var processedArgs = new Dictionary<string, ValueRef>();
				var macros = new Dictionary<string, ValueRef>();
				var macroStorageLocation = new StackValue(-1, $"macros", PrimitiveTypeSpecifier.Compound);

				foreach (var (param, val) in funcType.Parameters.Zip(args))
				{
					if (param.Modifiers.HasFlag(ParameterModifiers.Macro))
					{
						if (param.Type == PrimitiveTypeSpecifier.String)
						{
							// Exclude macros
							if (val.Value is not LiteralValue l)
							{
								throw new MacroStringError();
							}
							else
							{
								// Escape string
								macros.Add(param.Name, new LiteralValue(l.Value.ToString()));
							}
						}
						else
						{
							macros.Add(param.Name, val);
						}
					}
					else
					{
						processedArgs.Add(param.Name, val);
					}
				}

				if (processedArgs.Count != 0)
				{
					ctx.StoreCompound(new StackValue(-1, "args", PrimitiveTypeSpecifier.Compound), processedArgs, setEmpty: false);
				}

				if (macros.Count != 0)
				{
					processedMacros = ctx.StoreCompoundOrReturnConstant(macroStorageLocation, macros, setEmpty: false);
				}
			}

			return processedMacros;
		}
	}
}
