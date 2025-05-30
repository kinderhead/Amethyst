using Amethyst.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public class InitAssignmentNode(LocationRange loc, AbstractTypeDeclaration type, string name, AbstractExpression expr) : AbstractStatement(loc)
	{
		public readonly AbstractTypeDeclaration Type = type;
		public readonly string Name = name;
		public readonly AbstractExpression Expression = expr;
	}
}
