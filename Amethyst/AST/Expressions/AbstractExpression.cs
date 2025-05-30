using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Expressions
{
	public abstract class AbstractExpression(LocationRange loc) : Node(loc)
	{
	}

	public class AbstractLiteralExpression(LocationRange loc, NBTValue val) : AbstractExpression(loc)
	{
		public readonly NBTValue Value = val;
	}

	public class AbstractVariableExpression(LocationRange loc, string name) : AbstractExpression(loc)
	{
		public readonly string Name = name;
	}
}
