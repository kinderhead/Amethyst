using Geode;
using Geode.Types;

namespace Amethyst.AST
{
    public class NullFunction() : MethodNode(LocationRange.None, [], FunctionModifiers.None, new SimpleAbstractTypeSpecifier(LocationRange.None, "void"),
        $"amethyst:{GeodeBuilder.UniqueString}", $"amethyst:{GeodeBuilder.UniqueString}", [], new(LocationRange.None))
    {
        public override void Process(Compiler ctx, RootNode root)
        {
        }
    }
}