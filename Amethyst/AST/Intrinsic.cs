using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST
{
	public abstract class Intrinsic(NamespacedID id, FunctionType? type) : LiteralValue(new NBTString(id.ToString())), IFunctionLike
	{
		public NamespacedID ID => id;

		public override TypeSpecifier Type => FuncType;
		public FunctionType FuncType => type ?? new(FunctionModifiers.None, new VoidType(), []);

		public abstract IFunctionLike CloneWithType(FunctionType type);
		public abstract ValueRef Execute(FunctionContext ctx, params ValueRef[] args);
	}
}
