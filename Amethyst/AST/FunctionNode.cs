﻿using Amethyst.AST.Statements;
using Amethyst.Errors;
using Datapack.Net.Utils;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST
{
	public class FunctionNode(LocationRange loc, List<NamespacedID> tags, FunctionModifiers modifiers, AbstractTypeSpecifier ret, NamespacedID id, List<AbstractParameter> parameters, BlockNode body) : Node(loc), IRootChild
	{
		public readonly List<NamespacedID> Tags = tags;
		public readonly FunctionModifiers Modifiers = modifiers;
		public readonly AbstractTypeSpecifier ReturnType = ret;
		public readonly NamespacedID ID = id;
		public readonly List<AbstractParameter> Parameters = parameters;
		public readonly BlockNode Body = body;

		private FunctionType? funcType = null;
		public FunctionType GetFunctionType(Compiler ctx, bool recompute = false)
		{
			if (recompute || funcType is null)
			{
				funcType = new(Modifiers, ReturnType.Resolve(ctx, ID.GetContainingFolder()), Parameters.Select(i => new Parameter(i.Modifiers, i.Type.Resolve(ctx, ID.GetContainingFolder()), i.Name)));
			}

			return funcType;
		}

		public bool Compile(Compiler compiler, out FunctionContext? ctx)
		{
			ctx = null;

			var funcType = GetFunctionType(compiler);

			if (ID.Path.Any(char.IsUpper))
			{
				throw new InvalidNameError(ID.ToString());
			}

			ctx = new FunctionContext(compiler, (FunctionValue)compiler.IR.Symbols[ID].Value, Tags, Location);

			if (Body.Statements.Count == 0 || Body.Statements.Last() is not ReturnStatement)
			{
				if (funcType.ReturnType is VoidType)
				{
					Body.Add(new ReturnStatement(Location, null));
				}
				else
				{
					new MissingReturnError().Display(compiler, Location);
					return false;
				}
			}

			if (!Body.CompileWithErrorChecking(ctx))
			{
				return false;
			}

			ctx.Finish();

			return true;
		}

		public virtual void Process(Compiler ctx, RootNode root)
		{
			ctx.IR.AddSymbol(new(ID, Location, new FunctionValue(ID, GetFunctionType(ctx, true))));
			root.Functions.Add(this);
		}
	}

	public readonly record struct AbstractParameter(ParameterModifiers Modifiers, AbstractTypeSpecifier Type, string Name);
}
