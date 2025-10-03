using Amethyst.Geode.IR;
using Amethyst.Geode.Types;

namespace Amethyst.Geode
{
	public interface IFunctionLike
	{
		FunctionTypeSpecifier FuncType { get; }

		IFunctionLike CloneWithType(FunctionTypeSpecifier type);
		ValueRef AsMethod(ValueRef self, FunctionContext ctx);
	}
}
