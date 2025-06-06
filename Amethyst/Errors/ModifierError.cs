using System;
using Amethyst.AST;

namespace Amethyst.Errors
{
    public class ModifierError(LocationRange loc, string msg) : AmethystError(loc, msg)
    {
    }
}
