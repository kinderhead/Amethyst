using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST
{
	public abstract class Intrinsic(NamespacedID id, FunctionType? type) : LiteralValue(new NBTString(id.ToString()), type ?? new(FunctionModifiers.None, new VoidType(), [])), IFunctionLike
	{
		public NamespacedID ID => id;

		public FunctionType FuncType => (FunctionType)Type;

		public abstract IFunctionLike CloneWithType(FunctionType type);
		public abstract ValueRef Execute(FunctionContext ctx, params ValueRef[] args);
	}
}
