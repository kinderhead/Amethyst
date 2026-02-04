using Amethyst.AST.Expressions;
using Amethyst.AST.Statements;
using Datapack.Net.Utils;
using Geode;
using Geode.Types;

namespace Amethyst.AST
{
	public class MethodNode(LocationRange loc, List<NamespacedID> tags, FunctionModifiers modifiers, AbstractTypeSpecifier ret, NamespacedID id, List<AbstractParameter> parameters, BlockNode body) : FunctionNode(loc, tags, modifiers, ret, id, parameters, body)
	{
		public override void Process(Compiler ctx, RootNode root)
		{
			Parameters.Insert(0, new AbstractParameter(ParameterModifiers.Macro, new AbstractReferenceTypeSpecifier(Location, new SimpleAbstractTypeSpecifier(Location, string.Join('/', ID.ToString().Split('/')[..^1]))), "this"));
			base.Process(ctx, root);
		}

		protected override NamespacedID Mangle(TypeArray args) => base.Mangle(new(args.Types.Skip(1)));
	}

	public class ConstructorNode(LocationRange loc, List<NamespacedID> tags, FunctionModifiers modifiers, NamespacedID id, List<AbstractParameter> parameters, Expression? baseCall, BlockNode body) : MethodNode(loc, tags, modifiers, new SimpleAbstractTypeSpecifier(loc, id.ToString()), id, parameters, body)
	{
		public readonly Expression? BaseCall = baseCall;

		public override void Process(Compiler ctx, RootNode root)
		{
			var selfId = new NamespacedID(string.Join('/', ID.ToString().Split('/')[..^1]));
			var self = new SimpleAbstractTypeSpecifier(Location, selfId.ToString());
			var constructor = new FunctionNode(Location, Tags, Modifiers, self, selfId, Parameters, Body);

			constructor.Body.Prepend(new ConstructorInitStatement(Location, self, BaseCall));
			constructor.Body.Add(new ReturnStatement(Location, new VariableExpression(Location, "this")));
			constructor.Process(ctx, root);
		}

		protected override NamespacedID Mangle(TypeArray args) => args.Mangle(ID);
	}
}
