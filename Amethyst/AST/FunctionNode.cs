using Amethyst.AST.Statements;
using Amethyst.Errors;
using Amethyst.IR;
using Datapack.Net.Utils;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST
{
    public class FunctionNode(
        LocationRange loc,
        List<NamespacedID> tags,
        FunctionModifiers modifiers,
        AbstractTypeSpecifier ret,
        NamespacedID id,
        List<AbstractParameter> parameters,
        BlockNode body) : Node(loc), IRootChild
    {
        public readonly BlockNode Body = body;
        public readonly FunctionModifiers Modifiers = modifiers;
        public readonly List<AbstractParameter> Parameters = parameters;
        public readonly AbstractTypeSpecifier ReturnType = ret;
        public readonly List<NamespacedID> Tags = tags;

        private FunctionType? funcType;
        public NamespacedID ID { get; private set; } = id;

        public NamespacedID? Provides => null;

        public IEnumerable<NamespacedID> GetTypeDependencies(Compiler ctx) =>
            ReturnType.SoftResolve(ctx, ID.GetContainingFolder()).Concat(Parameters.SelectMany(i => i.Type.SoftResolve(ctx, ID.GetContainingFolder())));

        public virtual void Process(Compiler ctx, RootNode root) => ProcessAndGetFunc(ctx, root);

        public void SecondPass(Compiler ctx, RootNode root)
        {
        }

        public RawFunctionValue ProcessAndGetFunc(Compiler ctx, RootNode root)
        {
            var type = GetFunctionType(ctx, true);
            var realID = Mangle(type.ParameterTypes);

            // Keep the function's internal ID unchanged if it has no parameters.
            var func = Modifiers.HasFlag(FunctionModifiers.Virtual)
                ? new VirtualMethodValue(type.Parameters.Length == 0 ? ID : realID, type, Location)
                : new RawFunctionValue(type.Parameters.Length == 0 ? ID : realID, type, Location);

            if (ctx.IR.GetGlobal(ID) is OverloadedFunctionValue overload) overload.Add(func);
            else ctx.IR.AddSymbol(new(ID, Location, new OverloadedFunctionValue(ID).Add(func)));

            ctx.IR.AddSymbol(new(realID, Location, func));
            ID = realID;

            root.Functions.Add(this);

            return func;
        }

        public FunctionType GetFunctionType(Compiler ctx, bool recompute = false)
        {
            if (recompute || funcType is null)
            {
                funcType = new(Modifiers, ReturnType.Resolve(ctx, ID.GetContainingFolder()),
                    Parameters.Select(i =>
                        new Parameter(i.Modifiers, i.Type.Resolve(ctx, ID.GetContainingFolder()), i.Name)));
            }

            return funcType;
        }

        public bool Compile(Compiler compiler, out FunctionContext? ctx)
        {
            ctx = null;

            var type = GetFunctionType(compiler);

            ctx = new(compiler, (RawFunctionValue)compiler.IR.Symbols[ID].Value, Tags, Location);

            if (Body.Statements.Count == 0 || Body.Statements.Last() is not ReturnStatement)
            {
                if (type.ReturnType is VoidType) Body.Add(new ReturnStatement(Location, null));
                else
                {
                    new MissingReturnError().Display(compiler, Location);
                    return false;
                }
            }

            if (!Body.CompileWithErrorChecking(ctx)) return false;

            ctx.Finish();

            return true;
        }

        protected virtual NamespacedID Mangle(TypeArray args) => args.Mangle(ID);
    }

    public readonly record struct AbstractParameter(ParameterModifiers Modifiers, AbstractTypeSpecifier Type, string Name);
}