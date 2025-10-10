using Amethyst.Errors;
using Amethyst.IR.Types;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.Errors;
using Geode.Types;

namespace Amethyst.AST
{
	public class StructNode(LocationRange loc, NamespacedID id, AbstractTypeSpecifier? baseClass, Dictionary<string, AbstractTypeSpecifier> props, List<MethodNode> methods) : Node(loc), IRootChild
	{
		public readonly NamespacedID ID = id;
		public readonly Dictionary<string, AbstractTypeSpecifier> Properties = props;
		public readonly List<MethodNode> Methods = methods;
		public readonly AbstractTypeSpecifier? BaseClass = baseClass;

		public void Process(Compiler ctx, RootNode root)
		{
			var baseClass = BaseClass?.Resolve(ctx, ID.GetContainingFolder()) ?? PrimitiveTypeSpecifier.Compound;

			var props = new Dictionary<string, TypeSpecifier>();
			var methods = new Dictionary<string, FunctionTypeSpecifier>();

			foreach (var (k, v) in Properties)
			{
				props[k] = v.Resolve(ctx, ID.GetContainingFolder());
			}

			var selfType = new StructTypeSpecifier(ID, baseClass, props, methods);
			ctx.IR.AddType(new(ID, Location, selfType));

			var hasConstructor = false;

			foreach (var i in Methods)
			{
				i.Process(ctx, root);

				if (i is ConstructorNode)
				{
					hasConstructor = true;
				}
				else if (selfType.HierarchyMethod(i.ID.GetFile()) is FunctionTypeSpecifier other)
				{
					var otherIsVirtual = other.Modifiers.HasFlag(FunctionModifiers.Virtual);
					var thisIsVirtual = i.Modifiers.HasFlag(FunctionModifiers.Virtual);

					if (otherIsVirtual && !thisIsVirtual)
					{
						throw new MissingVirtualError(i.ID.GetFile());
					}
					else if (!otherIsVirtual)
					{
						throw new CannotOverrideError(i.ID.GetFile());
					}
				}
				else
				{
					var type = i.GetFunctionType(ctx);
					methods[i.ID.GetFile()] = type;

					if (i.Modifiers.HasFlag(FunctionModifiers.Virtual))
					{
						props[i.ID.GetFile()] = type;
					}
				}
			}

			if (ctx.IR.GetConstructorOrNull(selfType.BaseClass) is not null && !hasConstructor)
			{
				throw new MissingConstructorError(selfType.BaseClass.ToString());
			}

			if (Properties.Count != 0 && selfType.EffectiveType != NBTType.Compound)
			{
				throw new PropertyError(selfType.ToString());
			}
		}
	}
}
