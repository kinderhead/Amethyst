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

                // Minecraft throws an error if there are mismatched macro args, so we always reset it
                if (macros.Count != 0) macroStorageLocation.Store(new LiteralValue(new NBTCompound()), ctx);

                foreach (var (param, val) in FuncType.Parameters.Zip(args))
                {
                    if (param.Modifiers.HasFlag(AST.ParameterModifiers.Macro))
                    {
                        if (applyGuard && val.Type.MacroGuardStart != "" && val.Type.MacroGuardEnd != "")
                        {
                            //ctx.Call("amethyst:core/macro/guard", false, macroStorageLocation.Property(param.Name))
                        }
                        else macros.Add(param.Name, val);
                    }
                    else processedArgs.Add(param.Name, val);
                }

                if (processedArgs.Count != 0) ctx.StoreCompound(new StorageValue(GeodeBuilder.RuntimeID, "stack[-1].args", PrimitiveTypeSpecifier.Compound), processedArgs, setEmpty: false);
                if (macros.Count != 0) processedMacros = ctx.StoreCompoundOrReturnConstant(macroStorageLocation, macros, setEmpty: false); // Allow for macro guards
            }

            if (processedMacros is StorageValue s) ctx.Add(new FunctionCommand(ID, s.Storage, s.Path));
            else if (processedMacros is LiteralValue l) ctx.Add(new FunctionCommand(ID, (NBTCompound)l.Value));
            else ctx.Add(new FunctionCommand(ID));
        }

        public virtual FunctionValue CloneWithType(FunctionTypeSpecifier type) => new(ID, type);
    }
}
