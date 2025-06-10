using Amethyst.Codegen.IR;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Codegen.Functions
{
	public class PrintFunction() : CompileTimeFunction(new("amethyst", "print"), new DynamicFunctionTypeSpecifier(new VoidTypeSpecifier()))
	{
		public override Value Execute(FunctionContext ctx, IEnumerable<Value> parameters)
		{
			var formatter = new FormattedText();

			foreach (var i in parameters)
			{
				i.Format(formatter);
				formatter.Text(" ");
			}

			if (formatter.Count != 0) formatter.RemoveLast();
			formatter.Optimize();

			ctx.Add(new TellrawInstruction(ctx.CurrentLocator.Location, formatter));

			return new VoidValue();
		}
	}
}
