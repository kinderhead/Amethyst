using Amethyst.AST.Expressions;
using Amethyst.AST.Statements;
using Amethyst.IR.Types;
using Datapack.Net.Utils;
using Geode;
using Geode.Types;

namespace Amethyst.AST
{
    public class MethodNode(
        LocationRange loc,
        List<NamespacedID> tags,
        FunctionModifiers modifiers,
        AbstractTypeSpecifier ret,
        NamespacedID id,
        NamespacedID selfID,
        List<AbstractParameter> parameters,
        BlockNode body) : FunctionNode(loc, tags, modifiers, ret, id, parameters, body)
    {
        public readonly NamespacedID SelfID = selfID;

        public override void Process(Compiler ctx, RootNode root)
        {
            AbstractTypeSpecifier selfType = new SimpleAbstractTypeSpecifier(Location, SelfID.ToString());

            if (ctx.TypeHandler[SelfID].Type is StructType) selfType = new AbstractReferenceTypeSpecifier(Location, selfType);

            Parameters.Insert(0, new(ParameterModifiers.Macro, selfType, "this"));
            base.Process(ctx, root);
        }

        protected override NamespacedID Mangle(TypeArray args) => base.Mangle(new(args.Types.Skip(1)));
    }

    public class ConstructorNode(
        LocationRange loc,
        List<NamespacedID> tags,
        FunctionModifiers modifiers,
        NamespacedID id,
        List<AbstractParameter> parameters,
        Expression? baseCall,
        BlockNode body) : MethodNode(loc, tags, modifiers, new SimpleAbstractTypeSpecifier(loc, id.ToString()), id, id, parameters, body)
    {
        public readonly Expression? BaseCall = baseCall;

        public override void Process(Compiler ctx, RootNode root)
        {
            var self = new SimpleAbstractTypeSpecifier(Location, SelfID.ToString());
            var isClass = false;

            if (ctx.TypeHandler[SelfID].Type is ReferenceType)
            {
                isClass = true;
                Parameters.Insert(0, new(ParameterModifiers.Macro, self, "this"));
            }

            var constructor = new FunctionNode(Location, Tags, Modifiers, isClass ? new(Location, "builtin:void") : self, ID, Parameters, Body);
            constructor.Body.Prepend(new ConstructorInitStatement(Location, self, BaseCall));

            if (!isClass) constructor.Body.Add(new ReturnStatement(Location, new VariableExpression(Location, "this")));

            constructor.Process(ctx, root);
        }
    }
}