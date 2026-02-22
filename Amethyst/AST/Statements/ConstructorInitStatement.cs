using Amethyst.AST.Expressions;
using Amethyst.IR;
using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;
using System.Xml.Linq;

namespace Amethyst.AST.Statements
{
	public class ConstructorInitStatement(LocationRange loc, AbstractTypeSpecifier self, Expression? baseCall) : Statement(loc)
	{
		public readonly AbstractTypeSpecifier Self = self;
		public readonly Expression? BaseCall = baseCall;

		public override void Compile(FunctionContext ctx)
		{
			var actualThisType = Self.Resolve(ctx);
			StructType thisType;

			if (actualThisType is StructType t1)
			{
				thisType = t1;
			}
			else if (actualThisType is ReferenceType r1 && r1.Inner is StructType t2)
			{
				thisType = t2;
			}
			else
			{
				throw new InvalidTypeError(actualThisType.ToString(), "struct or class");
			}

			var self = actualThisType is StructType ? ctx.RegisterLocal("this", thisType, Location) : new ValueRef(ctx.GetVariable("this"));

			ValueRef? def = null;

			if (BaseCall is CallExpression call && actualThisType is ReferenceType)
			{
				call.Args.Insert(0, new VariableExpression(call.Location, "this"));
			}
			
			if (BaseCall is not null)
			{
				def = BaseCall?.Execute(ctx, null);
			}

			if (actualThisType is ReferenceType)
			{
				if (def is null)
				{
					ctx.Add(new StoreRefInsn(self, thisType.DefaultValue));
				}

				var typeIDProp = ctx.Add(new PropertyInsn(self, new LiteralValue(StructType.TypeIDProperty), PrimitiveType.String));
				typeIDProp.Type.AssignmentOverload(typeIDProp, new LiteralValue(thisType.ID.ToString()), ctx);
			}
			else
			{
				ctx.Add(new StoreInsn(self, def is null ? thisType.DefaultValue : ctx.ExplicitCast(def, thisType)));
			}

			if (thisType.BaseClass != thisType && thisType.BaseClass is StructType parent)
			{
				foreach (var (k, v) in thisType.Methods)
				{
					if (v.Modifiers.HasFlag(FunctionModifiers.Virtual))
					{
						var prop = ctx.GetProperty(self, k);
						prop.Type.AssignmentOverload(prop, thisType.DefaultPropertyValue(k)!, ctx);
					}
				}

				foreach (var (k, _) in thisType.Properties)
				{
					if (!parent.Properties.ContainsKey(k))
					{
						var prop = ctx.GetProperty(self, k);
						prop.Type.AssignmentOverload(prop, thisType.DefaultPropertyValue(k)!, ctx);
					}
				}
			}
		}
	}
}
