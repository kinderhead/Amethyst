using System;
using Amethyst.IR.Types;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.Errors;
using Geode.Values;

namespace Amethyst.IR
{
    public class OverloadedFunctionValue(NamespacedID id) : LiteralValue(new NBTString(id.ToString()))
    {
        public readonly NamespacedID ID = id;

        private readonly Dictionary<TypeArray, (LocationRange loc, NamespacedID id)> funcs = [];

        public OverloadedFunctionValue Add(FunctionValue val)
        {
            var existing = Get(val.FuncType.ParameterTypes);
            if (existing.Length != 0)
            {
                throw new RedefinedSymbolError(val.ID.ToString(), existing[0].loc);
            }

            funcs[val.FuncType.ParameterTypes] = (val.Location, val.ID);

            return this;
        }

        public (LocationRange loc, NamespacedID id)[] Get(TypeArray args)
        {
            List<(LocationRange loc, NamespacedID id)> ret = [];

            foreach (var (k, v) in funcs)
            {
                if (k.Length == args.Length)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        // TODO: Make this check implicit casting compatability somehow instead
                        if (!args[i].Implements(k[i]) && !(args[i] is ReferenceType ptr && ptr.Inner.Implements(k[i])) && !(k[i] is ReferenceType ptr2 && args[i].Implements(ptr2.Inner)))
                        {
                            goto end;
                        }
                    }

                    ret.Add(v);
                }

            end:;
            }

            return [.. ret];
        }
    }
}
