using Geode.Errors;
using System;

namespace Amethyst.Errors
{
    public class CannotIndexError(string type) : GeodeError($"Cannot index type {type}")
    {
        
    }
}
