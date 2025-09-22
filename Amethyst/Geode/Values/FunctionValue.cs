using Amethyst.AST;
using Amethyst.Errors;
using Amethyst.Geode.Types;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;

namespace Amethyst.Geode.Values
{
    public class FunctionValue(NamespacedID id, FunctionTypeSpecifier type) : LiteralValue(new NBTString(id.ToString()))
    {
        public readonly NamespacedID ID = id;
        public override TypeSpecifier Type => type;
        public override string ToString() => ID.ToString();
        public FunctionTypeSpecifier FuncType => (FunctionTypeSpecifier)Type;

        public virtual void Call(RenderContext ctx, ValueRef[] args, bool applyGuard = true)
        {
            Value? processedMacros = null;

            if (args.Length != FuncType.Parameters.Length) throw new MismatchedArgumentCountError(FuncType.Parameters.Length, args.Length);

            if (args.Length != 0)
            {
                var processedArgs = new Dictionary<string, ValueRef>();
                var macros = new Dictionary<string, ValueRef>();
                var macroStorageLocation = new StackValue(-1, $"macros{macroCounter++}", PrimitiveTypeSpecifier.Compound);

                foreach (var (param, val) in FuncType.Parameters.Zip(args))
                {
                    if (param.Modifiers.HasFlag(ParameterModifiers.Macro))
                    {
                        if (val.Value is MacroValue m && m.Type == PrimitiveTypeSpecifier.String) throw new InvalidTypeError("macro string", "string");
                        else if (applyGuard && param.Type.MacroGuardStart != "" && param.Type.MacroGuardEnd != "")
                        {
                            if (val.Value is ILiteralValue l)
                            {
                                macros.Add(param.Name, new LiteralValue($"{param.Type.MacroGuardStart}{(l.Value is NBTString str ? str.Value : l.Value.ToString())}{param.Type.MacroGuardEnd}"));
                            }
                            else
                            {
                                ctx.Call("amethyst:core/macro/guard", false, ReferenceTypeSpecifier.From(macroStorageLocation.Property(param.Name, param.Type)), new LiteralValue(param.Type.MacroGuardStart), val, new LiteralValue(param.Type.MacroGuardEnd));
                                macros.Add(param.Name, new VoidValue());
                            }
                        }
                        else macros.Add(param.Name, val);
                    }
                    else processedArgs.Add(param.Name, val);
                }

                if (processedArgs.Count != 0)
                {
                    if (macroCounter > 1) throw new NotImplementedException("Hyper-nested function calls with regular args are not supported right now");
                    ctx.StoreCompound(new StackValue(-1, "args", PrimitiveTypeSpecifier.Compound), processedArgs, setEmpty: false);
                }

                if (macros.Count != 0) processedMacros = ctx.StoreCompoundOrReturnConstant(macroStorageLocation, macros, setEmpty: false);

                macroCounter--;
            }

            if (processedMacros is StorageValue s) ctx.PossibleErrorChecker(new FunctionCommand(ID, s.Storage, s.Path),
                text => text
                    .Text($": Failed to run function ")
                    .Text(ID.ToString(), new FormattedText.Modifiers { Color = "red", SuggestCommand = $"/function {ID}", Underlined = true })
                    .Text($" with macro arguments: "),
                s
            );
            else if (processedMacros is LiteralValue l) ctx.PossibleErrorChecker(new FunctionCommand(ID, (NBTCompound)l.Value),
                text => text
                    .Text($": Failed to run function ")
                    .Text(ID.ToString(), new FormattedText.Modifiers { Color = "red", SuggestCommand = $"/function {ID} {l.Value}", Underlined = true })
                    .Text($" with macro arguments: "),
                l
            );
            else ctx.PossibleErrorChecker(new FunctionCommand(ID), $"Failed to run function \"{ID}\"");
        }

        public virtual FunctionValue CloneWithType(FunctionTypeSpecifier type) => new(ID, type);

        // Could just use a random string, but this makes it look nicer
        private static int macroCounter = 0;
    }
}
