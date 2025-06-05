using System;
using Amethyst.AST;

namespace Amethyst.Errors
{
    public class TagError(LocationRange loc, string msg) : AmethystError(loc, msg)
    {
    }
}
