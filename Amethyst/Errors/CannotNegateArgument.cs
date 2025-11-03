using Geode.Errors;
using System;

namespace Amethyst.Errors
{
    public class CannotNegateArgument(string arg) : GeodeError($"Cannot negate argument \"{arg}\" in target selector")
    {
        
    }
}
