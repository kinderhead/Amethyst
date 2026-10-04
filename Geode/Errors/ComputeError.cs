namespace Geode.Errors
{
    public class ComputeError(Value val) : GeodeError($"Cannot use {val} in compute equations");
}