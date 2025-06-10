using Amethyst.AST.Expressions;
using Amethyst.Codegen.IR;
using Datapack.Net.Function;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Codegen.Functions
{
	public abstract class CompileTimeFunction(NamespacedID id, DynamicFunctionTypeSpecifier type) : StaticFunctionValue(id, type)
	{
		public abstract Value Execute(FunctionContext ctx, IEnumerable<Value> parameters);
	}
}
