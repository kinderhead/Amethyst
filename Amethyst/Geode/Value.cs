using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Utils;

namespace Amethyst.Geode
{
    public abstract class Value
    {
        public abstract TypeSpecifier Type { get; }
        public bool IsLiteral => this is LiteralValue;

        public override string ToString() => "";
    }

    public class LiteralValue(NBTValue val) : Value
    {
        public readonly NBTValue Value = val;
        public override TypeSpecifier Type => new PrimitiveTypeSpecifier(Value.Type);
        public override string ToString() => Value.ToString();
    }

    public class VoidValue : Value
    {
        public override TypeSpecifier Type => new VoidTypeSpecifier();
    }

    public class StaticFunctionValue(NamespacedID id, FunctionTypeSpecifier type) : LiteralValue(new NBTString(id.ToString()))
    {
        public readonly NamespacedID ID = id;
        public override TypeSpecifier Type => type;
        public override string ToString() => ID.ToString();
        public FunctionTypeSpecifier FuncType => (FunctionTypeSpecifier)Type;
    }

    public class ScoreValue(IEntityTarget target, Score score) : Value
    {
        public readonly IEntityTarget Target = target;
        public readonly Score Score = score;
        public override string ToString() => $"@{Target.Get()}.{Score}";

        public override TypeSpecifier Type => PrimitiveTypeSpecifier.Int;
    }

    public abstract class LValue : Value
    {

    }
}
