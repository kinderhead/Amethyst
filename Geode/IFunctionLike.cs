using Geode.Types;

namespace Geode
{
	public interface IFunctionLike
	{
		FunctionTypeSpecifier FuncType { get; }

		IFunctionLike CloneWithType(FunctionTypeSpecifier type);
	}
}
