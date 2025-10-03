namespace Datapack.Net.CubeLib
{
	[AttributeUsage(AttributeTargets.Property)]
	public class MCProperty(string name) : Attribute
	{
		public readonly string Name = name;
	}
}
