using System;
using Amethyst.Codegen.IR;
using Datapack.Net.Data;

namespace Amethyst.Codegen
{
    public class ReturnOrStore
    {
        public readonly FunctionContext Ctx;
        public readonly TypeSpecifier Type;

        public Value EffectiveValue => dest ?? value ?? throw new InvalidOperationException();

        private Value? value = null;
        private MutableValue? dest = null;

        public ReturnOrStore(FunctionContext ctx, TypeSpecifier type)
        {
            Ctx = ctx;
            Type = type;
        }

        public ReturnOrStore(FunctionContext ctx, MutableValue dest)
        {
            Ctx = ctx;
            this.dest = dest;
            Type = dest.Type;
        }

        public void Store(Value val)
        {
            if (dest is not null) dest.Store(Ctx, val);
            else value = val;
        }

        public void Store(Value val, NBTNumberType type)
        {
            if (dest is not null) dest.Store(Ctx, val, type);
            else
            {
                dest = Ctx.AllocTemp(Type);
                dest.Store(Ctx, val, type);
            }
        }

        public StorageValue RequireStorage()
        {
            if (dest is StorageValue s) return s;
            else if (dest is null) return (StorageValue)(dest = Ctx.AllocTemp(Type));
            else throw new NotImplementedException();
        }
    }
}
