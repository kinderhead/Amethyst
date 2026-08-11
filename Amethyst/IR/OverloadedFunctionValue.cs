using Amethyst.Errors;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Values;

namespace Amethyst.IR
{
    public class OverloadedFunctionValue(NamespacedID id) : LiteralValue(new NBTString(id.ToString())), IMinimalFunction
    {
        public readonly NamespacedID ID = id;
        private readonly Dictionary<TypeArray, RawFunctionValue> funcs = [];

        public ValueRef CallBehavior(FunctionContext ctx, params ValueRef[] args) => Get(args).CallBehavior(ctx, args);

        public RawFunctionValue Get(TypeArray types) => funcs.GetValueOrDefault(types) ?? throw new NoOverloadError(ID, types);

        public OverloadedFunctionValue Add(RawFunctionValue val)
        {
            if (funcs.TryGetValue(val.FuncType.ParameterTypes, out var existing)) throw new RedefinedSymbolError(val.FuncType.ToString(ID.ToString()), existing.Location);

            funcs[val.FuncType.ParameterTypes] = val;

            return this;
        }

        public RawFunctionValue[] GetAll(ValueRef[] args)
        {
            List<RawFunctionValue> ret = [];

            foreach (var (k, v) in funcs)
            {
                if (k.Length == args.Length)
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        if (!FunctionContext.CanImplicitCast(args[i], k[i])) goto end;
                    }

                    ret.Add(v);
                }

                end: ;
            }

            return [.. ret];
        }

        public RawFunctionValue Get(ValueRef[] args)
        {
            var types = TypeArray.From(args);
            var options = GetAll(args);

            if (options.Length == 0) throw new NoOverloadError(ID, types);

            var option = options[0];

            if (options.Length > 1)
            {
                // Prioritize exact type matches
                if (funcs.GetValueOrDefault(types) is not { } option1) throw new AmbiguousOverloadError(ID, types);
                option = option1;
            }

            return option;
        }
    }
}