using System.Collections.Generic;

namespace Datapack.Net.SourceGenerator
{
	public readonly record struct MCFunction
	{
		public readonly (string, string)[] Arguments;
		public readonly string FullyQualifiedName;
		public readonly bool Macro;
		public readonly string Name;
		public readonly string ReturnType;

		public MCFunction(string name, string fullName, string returnType, List<(string, string)> args, bool macro)
		{
			Name = name;
			Arguments = [.. args];
			Macro = macro;
			FullyQualifiedName = fullName;
			ReturnType = returnType;
		}
	}
}