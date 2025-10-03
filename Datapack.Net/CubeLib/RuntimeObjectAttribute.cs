using System.Reflection;

namespace Datapack.Net.CubeLib
{
	[AttributeUsage(AttributeTargets.Class)]
	public class RuntimeObjectAttribute(string name, bool implementCleanup = true) : Attribute
	{
		public readonly string Name = name;
		public readonly bool ImplementCleanup = implementCleanup;

		public static RuntimeObjectAttribute Get<T>() => typeof(T).GetCustomAttribute<RuntimeObjectAttribute>() ?? throw new InvalidOperationException("Function does not have the DeclareMC attribute");
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class RuntimePropertyAttribute(string name) : Attribute
	{
		public readonly string Name = name;
	}
}
