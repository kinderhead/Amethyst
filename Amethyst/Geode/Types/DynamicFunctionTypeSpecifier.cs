using Amethyst.AST;

namespace Amethyst.Geode.Types
{
	public class DynamicFunctionTypeSpecifier(TypeSpecifier returnType) : FunctionTypeSpecifier(FunctionModifiers.None, returnType, [])
	{
		protected override bool EqualsImpl(TypeSpecifier obj) => base.EqualsImpl(obj) && obj is DynamicFunctionTypeSpecifier;
	}
}
