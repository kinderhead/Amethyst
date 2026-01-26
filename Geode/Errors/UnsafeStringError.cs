using System;

namespace Geode.Errors
{
    public class UnsafeStringError() : GeodeError($"String is not a valid unsafe_string")
    {
        
    }
}
