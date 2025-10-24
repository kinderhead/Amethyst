using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.Types;

namespace Geode.Values
{
	public class FunctionValue(NamespacedID id, FunctionType type) : LiteralValue(new NBTString(id.ToString())), IFunctionLike
	{
		public readonly NamespacedID ID = id;
		public override TypeSpecifier Type => type;
		public override string ToString() => ID.ToString();
		public FunctionType FuncType => (FunctionType)Type;

		public virtual void Call(RenderContext ctx, IValueLike[] args) => Call(ctx, ID, FuncType, args);
		public virtual IFunctionLike CloneWithType(FunctionType type) => new FunctionValue(ID, type);

		public static void Call(RenderContext ctx, NamespacedID id, FunctionType funcType, IValueLike[] args)
		{
			var processedMacros = SetArgsAndGetMacros(ctx, funcType, args);

			if (processedMacros is StorageValue s)
			{
				ctx.PossibleErrorChecker(new FunctionCommand(id, s.Storage, s.Path),
					text => text
						.Text($": Failed to run function ")
						.Text(id.ToString(), new FormattedText.Modifiers { Color = "red", SuggestCommand = $"/function {id}", Underlined = true })
						.Text($" with macro arguments: "),
					s
				);
			}
			else if (processedMacros is LiteralValue l)
			{
				ctx.PossibleErrorChecker(new FunctionCommand(id, (NBTCompound)l.Value),
					text => text
						.Text($": Failed to run function ")
						.Text(id.ToString(), new FormattedText.Modifiers { Color = "red", SuggestCommand = $"/function {id} {l.Value}", Underlined = true })
						.Text($" with macro arguments: "),
					l
				);
			}
			else
			{
				ctx.PossibleErrorChecker(new FunctionCommand(id), $"Failed to run function \"{id}\"");
			}
		}

		public static IValue? SetArgsAndGetMacros(RenderContext ctx, FunctionType funcType, IValueLike[] args)
		{
			IValue? processedMacros = null;

			if (args.Length != funcType.Parameters.Length)
			{
				throw new MismatchedArgumentCountError(funcType.Parameters.Length, args.Length);
			}

			if (args.Length != 0)
			{
				var processedArgs = new Dictionary<string, IValueLike>();
				var macros = new Dictionary<string, IValueLike>();
				var macroStorageLocation = new StackValue(-1, ctx.Builder.RuntimeID, $"macros", PrimitiveType.Compound);

				foreach (var (param, val) in funcType.Parameters.Zip(args))
				{
					if (param.Modifiers.HasFlag(ParameterModifiers.Macro))
					{
						if (param.Type == PrimitiveType.String)
						{
							// Exclude macros
							if (val.Expect() is not LiteralValue l)
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
					ctx.StoreCompound(new StackValue(-1, ctx.Builder.RuntimeID, "args", PrimitiveType.Compound), processedArgs, setEmpty: false);
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
