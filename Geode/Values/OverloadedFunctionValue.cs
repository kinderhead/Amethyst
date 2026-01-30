using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.Types;
using System;

namespace Geode.Values
{
    public class OverloadedFunctionValue(NamespacedID id) : LiteralValue(new NBTString(id.ToString()))
    {
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

            foreach (var i in funcs)
            {
                
            }

            return [.. ret];
        }
    }
}
