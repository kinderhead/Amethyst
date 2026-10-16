namespace Geode.Errors
{
    public class UnsafeStringError(string val) : GeodeError($"\"{val}\" is not a valid unsafe_string");

    public class QStringError(string val) : GeodeError($"\"{val}\" is not a valid qstring");
}