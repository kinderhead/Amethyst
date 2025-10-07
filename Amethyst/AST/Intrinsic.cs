using Amethyst.AST.Intrinsics;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST
{
	public abstract class Intrinsic(NamespacedID id, FunctionTypeSpecifier? type) : LiteralValue(new NBTString(id.ToString())), IFunctionLike
	{
		public NamespacedID ID => id;

		public override TypeSpecifier Type => FuncType;
		public FunctionTypeSpecifier FuncType => type ?? new(FunctionModifiers.None, new VoidTypeSpecifier(), []);

		public ValueRef AsMethod(ValueRef self, FunctionContext ctx) => new IntrinsicMethod(this, self);

		public abstract IFunctionLike CloneWithType(FunctionTypeSpecifier type);
		public abstract ValueRef Execute(FunctionContext ctx, params ValueRef[] args);
	}
}
