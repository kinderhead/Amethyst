using Amethyst.AST;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Codegen
{
	public readonly record struct GlobalSymbol(NamespacedID ID, TypeSpecifier Type, LocationRange Location, Value Value);
	public readonly record struct LocalSymbol(string Name, LocationRange Location, Value Value);
}
