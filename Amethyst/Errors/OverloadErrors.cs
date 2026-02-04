using Datapack.Net.Utils;
using Geode;
using Geode.Errors;
using System;

namespace Amethyst.Errors
{
    public class NoOverloadError(NamespacedID id, TypeArray types) : GeodeError($"No overload for function {id} has arguments ({types})")
    {
        
    }

    public class AmbiguousOverloadError(NamespacedID id, TypeArray types) : GeodeError($"Ambiguous overload for function {id} with arguments ({types})")
    {

    }
}
