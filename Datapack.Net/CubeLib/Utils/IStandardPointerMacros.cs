namespace Datapack.Net.CubeLib.Utils
{
	public interface IStandardPointerMacros
	{
		KeyValuePair<string, object>[] StandardMacros(KeyValuePair<string, object>[]? extras = null, string postfix = "");
	}
}
