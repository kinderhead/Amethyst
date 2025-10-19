using Geode.Types;

namespace Geode
{
	public interface IFunctionLike : IValue
	{
		FunctionType FuncType { get; }

		IFunctionLike CloneWithType(FunctionType type);
	}
}
