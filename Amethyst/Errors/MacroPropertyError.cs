using Geode.Errors;
using System;

namespace Amethyst.Errors
{
    public class MacroPropertyError() : GeodeError("Cannot index or get property of macro value. Try assigning to another variable first")
    {
        
    }
}
