using Amethyst.IR;
using Amethyst.IR.Types;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;

namespace Amethyst.AST.Statements
{
    public class ConstructorInitStatement(LocationRange loc, AbstractTypeSpecifier self, Expression? baseCall) : Statement(loc)
    {
        public readonly AbstractTypeSpecifier Self = self;
        public readonly Expression? BaseCall = baseCall;

        public override void Compile(FunctionContext ctx)
        {
            var thisType = (StructType)Self.Resolve(ctx);

            var self = ctx.RegisterLocal("this", thisType);

            var def = BaseCall?.Execute(ctx, null);
            var defType = def?.Type;
            ctx.Add(new StoreInsn(self, def is null ? thisType.DefaultValue : ctx.ExplicitCast(def, thisType)));

            if (defType is not null && defType != thisType && defType is StructType parent)
            {
                foreach (var (k, v) in thisType.Methods)
                {
                    if (v.Modifiers.HasFlag(FunctionModifiers.Virtual))
                    {
                        var prop = ctx.GetProperty(self, k);
                        prop.Type.AssignmentOverload(prop, thisType.DefaultPropertyValue(k)!, ctx);
                    }
                }

                foreach (var (k, _) in thisType.Properties)
                {
                    if (!parent.Properties.ContainsKey(k))
                    {
                        var prop = ctx.GetProperty(self, k);
                        prop.Type.AssignmentOverload(prop, thisType.DefaultPropertyValue(k)!, ctx);
                    }
                }
            }
        }
    }
}
