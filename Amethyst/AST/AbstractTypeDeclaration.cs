using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST
{
	public abstract class AbstractTypeDeclaration(LocationRange loc) : Node(loc)
	{

	}

	public class SimpleAbstractTypeDeclaration(LocationRange loc, string type) : AbstractTypeDeclaration(loc)
	{
		public readonly string Type = type;
	}
}
