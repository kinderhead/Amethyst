using Amethyst.AST;

namespace Amethyst.Geode.Types
{
	public class DynamicFunctionTypeSpecifier(TypeSpecifier returnType) : FunctionTypeSpecifier(FunctionModifiers.None, returnType, [])
	{
		protected override bool AreEqual(TypeSpecifier obj) => base.AreEqual(obj) && obj is DynamicFunctionTypeSpecifier;
	}
}
