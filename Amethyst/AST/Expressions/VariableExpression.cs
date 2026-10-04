using Amethyst.IR;
using Amethyst.IR.Types;
using Geode;
using Geode.IR;
using Geode.Values;

namespace Amethyst.AST.Expressions
{
	public class VariableExpression(LocationRange loc, string name) : Expression(loc), IMethodHolder
	{
		public readonly string Name = name;
		private ValueRef? implicitThis;

		public Expression? GetThis(FunctionContext ctx) => implicitThis is { } self
			? new ValueRefExpression(Location, self)
			: null;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			implicitThis = null;
			var val = ctx.GetLocalVariableOrNull(Name);

			if (val is null && ctx.GetLocalVariableOrNull("this") is { } self)
			{
				var selfRef = new ValueRef(self);

				if (self.Type.HasProperty(Name) is not null) return GetImplicitProperty(ctx, selfRef, expected);

				if (ctx.GetMethodOrNull(selfRef, Name) is { } method)
				{
					implicitThis = selfRef;
					return method;
				}

				if (Name is not ("true" or "false") && self.Type.DefaultPropertyType is not null) return GetImplicitProperty(ctx, selfRef, expected);
			}

			val ??= ctx.GetVariable(Name);
			
			if (ctx.InForkingExecute && val is Variable v)
			{
				v.ForceStack = true;
			}

			return new(val);
		}

		private ValueRef GetImplicitProperty(FunctionContext ctx, ValueRef self, TypeSpecifier? expected)
		{
			var property = ctx.GetProperty(self, Name);

			if (expected is null && property.Type is ReferenceType ptr) property = ctx.ImplicitCast(property, ptr.Inner);

			return property;
		}
	}
}