namespace Amethyst.Errors
{
    public class ReferenceError(string val) : AmethystError($"Cannot make a reference to {val}")
    {

    }
}
