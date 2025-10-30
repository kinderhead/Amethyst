using System;

namespace Geode.Errors
{
    public class ForkedElseError() : GeodeError("Else statements for branching execute statements is not supported yet")
    {
        
    }
}
