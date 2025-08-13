using Datapack.Net.Data;
using Datapack.Net.Utils;

namespace Amethyst.Geode
{
    public abstract class Value
    {
        public abstract TypeSpecifier Type { get; }
        public virtual string Name => "";
        public bool IsLiteral => this is LiteralValue;
    }

    public class LiteralValue(NBTValue val) : Value
    {
        public readonly NBTValue Value = val;
        public override TypeSpecifier Type => new PrimitiveTypeSpecifier(Value.Type);
        public override string Name => Value.ToString();
    }

    public class VoidValue : Value
    {
        public override TypeSpecifier Type => new VoidTypeSpecifier();
    }

    public class StaticFunctionValue(NamespacedID id, FunctionTypeSpecifier type) : LiteralValue(new NBTString(id.ToString()))
    {
        public readonly NamespacedID ID = id;
        public override TypeSpecifier Type => type;
        public override string Name => ID.ToString();
        public FunctionTypeSpecifier FuncType => (FunctionTypeSpecifier)Type;
    }
}
