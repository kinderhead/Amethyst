using System;
using Amethyst.AST;

namespace Amethyst.Errors
{
    public class PropertyError(LocationRange loc, string type, string prop) : AmethystError(loc, $"{type} does not have property with name \"{prop}\"")
    {
    }
}
