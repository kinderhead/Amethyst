using Datapack.Net.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Datapack.Net.NumberProviders
{
    public abstract class BinaryOperationProvider(NumberProvider left, NumberProvider right, NamespacedID id) : NumberProvider(id)
    {
        public readonly NumberProvider Left = left;

        public readonly NumberProvider Right = right;

        public override ProviderNumberType NumberType => Left.NumberType == ProviderNumberType.Float || Right.NumberType == ProviderNumberType.Float
            ? ProviderNumberType.Float
            : ProviderNumberType.Int;

        public abstract bool Associative { get; }

        protected override JObject InnerRender() => Associative
            ? new()
            {
                ["inputs"] = new JArray([ToType(Left).Render(), ToType(Right).Render()])
            }
            : new()
            {
                ["left"] = ToType(Left).Render(),
                ["right"] = ToType(Right).Render()
            };
    }

    public class AddProvider(NumberProvider left, NumberProvider right, NamespacedID id) : BinaryOperationProvider(left, right, id)
    {
        public override NamespacedID Type => "minecraft:add";
        public override bool Associative => true;
    }

    public class SubProvider(NumberProvider left, NumberProvider right, NamespacedID id) : BinaryOperationProvider(left, right, id)
    {
        public override NamespacedID Type => "minecraft:sub";
        public override bool Associative => false;
    }

    public class MulProvider(NumberProvider left, NumberProvider right, NamespacedID id) : BinaryOperationProvider(left, right, id)
    {
        public override NamespacedID Type => "minecraft:mul";
        public override bool Associative => true;
    }

    public class DivProvider(NumberProvider left, NumberProvider right, NamespacedID id) : BinaryOperationProvider(left, right, id)
    {
        public override NamespacedID Type => "minecraft:div";
        public override bool Associative => false;
    }

    public class ModProvider(NumberProvider left, NumberProvider right, NamespacedID id) : BinaryOperationProvider(left, right, id)
    {
        public override NamespacedID Type => "minecraft:mod";
        public override bool Associative => false;
    }

    public class PowProvider(NumberProvider left, NumberProvider right, NamespacedID id) : BinaryOperationProvider(left, right, id)
    {
        public override NamespacedID Type => "minecraft:pow";
        public override bool Associative => true;
    }
}
