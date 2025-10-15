using Geode.Types;

namespace Geode
{
	public interface IFunctionLike
	{
		FunctionType FuncType { get; }

		IFunctionLike CloneWithType(FunctionType type);
	}
}
