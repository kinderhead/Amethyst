using Amethyst.AST.Expressions;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.Values;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST
{
	public abstract class Intrinsic(string name) : LiteralValue(new NBTString($"builtin:{name}"))
	{
		public readonly string Name = name;
		public NamespacedID ID => new("builtin", Name);

		public override TypeSpecifier Type => new FunctionTypeSpecifier(FunctionModifiers.None, new VoidTypeSpecifier(), []);
		
		public abstract ValueRef Execute(FunctionContext ctx, params IEnumerable<Expression> args);
	}
}
