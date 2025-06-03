using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST
{
	public abstract class AbstractTypeSpecifier(LocationRange loc) : Node(loc)
	{
		public TypeSpecifier Resolve(FunctionContext ctx) => Resolve(ctx.Compiler);
		public abstract TypeSpecifier Resolve(Compiler ctx);
	}

	public class SimpleAbstractTypeSpecifier(LocationRange loc, string type) : AbstractTypeSpecifier(loc)
	{
		public readonly string Type = type;

		public override TypeSpecifier Resolve(Compiler ctx)
		{
			switch (Type)
			{
				case "void":
					return new VoidTypeSpecifier();
				case "bool":
					return new PrimitiveTypeSpecifier(NBTType.Boolean);
				case "byte":
					return new PrimitiveTypeSpecifier(NBTType.Byte);
				case "short":
					return new PrimitiveTypeSpecifier(NBTType.Short);
				case "int":
					return new PrimitiveTypeSpecifier(NBTType.Int);
				case "long":
					return new PrimitiveTypeSpecifier(NBTType.Long);
				case "float":
					return new PrimitiveTypeSpecifier(NBTType.Float);
				case "double":
					return new PrimitiveTypeSpecifier(NBTType.Double);
				case "string":
					return new PrimitiveTypeSpecifier(NBTType.String);
				default:
					break;
			}

			throw new UnknownTypeError(Location, Type);
		}
	}
}
