using Datapack.Net.Utils;
using Geode;
using Geode.Errors;

namespace Amethyst.Errors
{
    public class NoOverloadError(NamespacedID id, TypeArray types) : GeodeError($"No overload for function {id} has arguments ({types})");

    public class AmbiguousOverloadError : GeodeError
    {
        public AmbiguousOverloadError(NamespacedID id, TypeArray types) : base($"Ambiguous overload for function {id} with arguments ({types})")
        {
        }

        public AmbiguousOverloadError(NamespacedID id) : base($"Ambiguous function {id}")
        {
        }
    }
}