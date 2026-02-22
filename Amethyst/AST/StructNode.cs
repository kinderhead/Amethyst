using Amethyst.Errors;
using Amethyst.IR.Types;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.Errors;
using Geode.Types;
using System.Collections.ObjectModel;

namespace Amethyst.AST
{
	public enum ContainerType
	{
		Struct,
		Class,
		Entity
	}

	public class StructNode(ContainerType type, LocationRange loc, NamespacedID id, AbstractTypeSpecifier? baseClass, Dictionary<string, AbstractTypeSpecifier> props, List<MethodNode> methods) : Node(loc), IRootChild
	{
		public readonly ContainerType Type = type;
		public readonly NamespacedID ID = id;
		public readonly Dictionary<string, AbstractTypeSpecifier> Properties = props;
		public readonly List<MethodNode> Methods = methods;
		public readonly AbstractTypeSpecifier? BaseClass = baseClass;

		public void Process(Compiler ctx, RootNode root)
		{
			var baseClass = BaseClass?.Resolve(ctx, ID.GetContainingFolder()) ?? (Type == ContainerType.Entity ? EntityType.Dummy : PrimitiveType.Compound);

			if (baseClass is ReferenceType r)
			{
				baseClass = r.Inner;
			}

			var props = new Dictionary<string, TypeSpecifier>();
			var methods = new Dictionary<string, FunctionType>();

			foreach (var (k, v) in Properties)
			{
				props[k] = v.Resolve(ctx, ID.GetContainingFolder());

				if (Type != ContainerType.Class && props[k] is ReferenceType && props[k] is not WeakReferenceType)
				{
					throw new ReferencePropertyError(k);
				}
				else if (ReservedProperties.Contains(k))
				{
					throw new ReservedNameError(k);
				}
			}

			var selfType = Type switch
			{
				ContainerType.Struct or ContainerType.Class => new StructType(ID, baseClass, props, methods, Type == ContainerType.Class),
				ContainerType.Entity => new EntityType(ID, baseClass, props, methods),
				_ => throw new NotImplementedException()
			};

			if (Type == ContainerType.Class)
			{
				ctx.IR.AddType(new(ID, Location, new ReferenceType(selfType, false)));
			}
			else
			{
				ctx.IR.AddType(new(ID, Location, selfType));
			}

			ConstructorNode? constructor = null;

			foreach (var i in Methods)
			{
				i.Process(ctx, root);

				if (i is ConstructorNode c)
				{
					constructor = c;
				}
				else
				{
					var type = i.GetFunctionType(ctx);
					methods[i.ID.GetFile()] = type;

					var thisIsVirtual = i.Modifiers.HasFlag(FunctionModifiers.Virtual);

					if (thisIsVirtual && Type == ContainerType.Entity)
					{
						throw new VirtualEntityMethodError();
					}

					if (selfType.HierarchyMethod(i.ID.GetFile())?.Type is FunctionType other)
					{
						var otherIsVirtual = other.Modifiers.HasFlag(FunctionModifiers.Virtual);

						if (otherIsVirtual && !thisIsVirtual)
						{
							throw new MissingVirtualError(i.ID.GetFile());
						}
						else if (!otherIsVirtual)
						{
							throw new CannotOverrideError(i.ID.GetFile());
						}
						else if (other.ReturnType != type.ReturnType || other.Parameters.Length != type.Parameters.Length || !other.Parameters.Skip(1).Zip(type.Parameters.Skip(1)).All(i => i.First == i.Second))
						{
							throw new InvalidOverrideSignatureError(i.ID.GetFile());
						}
					}
					else if (thisIsVirtual)
					{
						props[i.ID.GetFile()] = type;
					}
				}
			}

			if (ctx.IR.GetConstructorOrNull(selfType.BaseClass) is not null && (constructor is null || constructor.BaseCall is null))
			{
				throw new MissingConstructorError(selfType.BaseClass.ToString());
			}

			if (Properties.Count != 0 && Type == ContainerType.Struct && selfType.EffectiveType != NBTType.Compound)
			{
				throw new PropertyError(selfType.ToString());
			}
		}

		public static readonly ReadOnlySet<string> ReservedProperties = [StructType.TypeIDProperty];
	}
}
