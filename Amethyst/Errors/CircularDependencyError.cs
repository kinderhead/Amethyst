using Geode.Errors;

namespace Amethyst.Errors
{
    public class CircularDependencyError(string id) : GeodeError($"\"{id}\" has a circular dependency issue");
}