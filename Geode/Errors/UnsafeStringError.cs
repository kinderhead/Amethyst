using System;

namespace Geode.Errors
{
    public class UnsafeStringError() : GeodeError($"String is not a valid unsafe_string")
    {
        
    }

    public class QStringError() : GeodeError($"String is not a valid qstring")
    {

    }
}
