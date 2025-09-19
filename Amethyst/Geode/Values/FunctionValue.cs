using Amethyst.AST;
using Amethyst.Errors;
using Amethyst.Geode.Types;
using Datapack.Net.Data;
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
                var macroStorageLocation = new StorageValue(GeodeBuilder.RuntimeID, "stack[-1].macros", PrimitiveTypeSpecifier.Compound);

                var alreadyResetMacros = false;

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
                                if (!alreadyResetMacros)
                                {
                                    macroStorageLocation.Store(new LiteralValue(new NBTCompound()), ctx);
                                    alreadyResetMacros = true;
                                }

                                ctx.Call("amethyst:core/macro/guard", false, ReferenceTypeSpecifier.From(macroStorageLocation.Property(param.Name, param.Type)), new LiteralValue(param.Type.MacroGuardStart), val, new LiteralValue(param.Type.MacroGuardEnd));
                                macros.Add(param.Name, new VoidValue());
                            }
                        }
                        else macros.Add(param.Name, val);
                    }
                    else processedArgs.Add(param.Name, val);
                }

                if (processedArgs.Count != 0) ctx.StoreCompound(new StorageValue(GeodeBuilder.RuntimeID, "stack[-1].args", PrimitiveTypeSpecifier.Compound), processedArgs, setEmpty: false);

                // Minecraft throws an error if there are mismatched macro args, so we always reset it
                if (macros.Count != 0) processedMacros = ctx.StoreCompoundOrReturnConstant(macroStorageLocation, macros, !alreadyResetMacros);
            }

            if (processedMacros is StorageValue s) ctx.Add(new FunctionCommand(ID, s.Storage, s.Path));
            else if (processedMacros is LiteralValue l) ctx.Add(new FunctionCommand(ID, (NBTCompound)l.Value));
            else ctx.Add(new FunctionCommand(ID));
        }

        public virtual FunctionValue CloneWithType(FunctionTypeSpecifier type) => new(ID, type);
    }
}
