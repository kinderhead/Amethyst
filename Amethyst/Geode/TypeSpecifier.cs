using Amethyst.AST;
using Amethyst.Errors;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode
{
	public abstract class TypeSpecifier : ICloneable
	{
		public abstract bool Operable { get; }
		public virtual bool IsList => false;
		public virtual IEnumerable<TypeSpecifier> Subtypes => [];
		public virtual string BasePath => "minecraft";
		public virtual TypeSpecifier BaseClass => PrimitiveTypeSpecifier.Compound;

		// TODO: Make this not throw an error
		public virtual NBTType EffectiveType => throw new InvalidOperationException(ToString());
		public bool ShouldStoreInScore => EffectiveType == NBTType.Int || EffectiveType == NBTType.Boolean;

		public NBTNumberType? EffectiveNumberType => Enum.IsDefined((NBTNumberType)EffectiveType) ? (NBTNumberType)EffectiveType : null;

		//public virtual bool IsAssignableTo(TypeSpecifier other) => this == other;
		public virtual TypeSpecifier? Property(string name) => null;
		public abstract LiteralValue DefaultValue { get; }

		public override bool Equals(object? obj)
		{
			if (obj is not TypeSpecifier other) return false;
			else return GetEquatableType().AreEqual(other.GetEquatableType());
		}

		public virtual bool Implements(TypeSpecifier other)
		{
			if (this == other) return true;
			else if (this == BaseClass) return false;
			else return BaseClass.Implements(other);
		}

		public virtual bool ConstraintSatisfiedBy(TypeSpecifier other)
		{
			return other.GetType() == GetType()
				&& Subtypes.Count() == other.Subtypes.Count()
				&& Subtypes.Zip(other.Subtypes).All(i => i.First.ConstraintSatisfiedBy(i.Second));
		}

		public TypeSpecifier ApplyGeneric(TypeSpecifier other)
		{
			if (!ConstraintSatisfiedBy(other)) return this;

			var type = (TypeSpecifier)Clone();
			var typeMap = new Dictionary<string, TypeSpecifier>();

			type.ApplyGeneric(other, typeMap);

			return type;
		}

		protected virtual void ApplyGeneric(TypeSpecifier other, Dictionary<string, TypeSpecifier> typeMap)
		{
			foreach (var (first, second) in Subtypes.Zip(other.Subtypes))
			{
				first.ApplyGeneric(second, typeMap);
			}
		}

		public override int GetHashCode() => ToString().GetHashCode();

		public abstract override string ToString();
		protected abstract bool AreEqual(TypeSpecifier obj);
		public virtual TypeSpecifier GetEquatableType() => this;
		public abstract object Clone();

		public static bool operator ==(TypeSpecifier a, TypeSpecifier b) => a.Equals(b);
		public static bool operator !=(TypeSpecifier a, TypeSpecifier b) => !a.Equals(b);
	}

	public class VoidTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;
		public override LiteralValue DefaultValue => new(0, this);

		protected override bool AreEqual(TypeSpecifier obj) => obj is VoidTypeSpecifier;
		public override string ToString() => "void";
		public override object Clone() => new VoidTypeSpecifier();
	}

	public class VarTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;
		public override LiteralValue DefaultValue => throw new InvalidTypeError("var");
		public override string ToString() => "var";
		protected override bool AreEqual(TypeSpecifier obj) => obj is VarTypeSpecifier;
		public override object Clone() => new VarTypeSpecifier();
	}

	public class AnyTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;
		public override LiteralValue DefaultValue => new(new NBTCompound());
		public override NBTType EffectiveType => NBTType.Compound;
		public override string ToString() => "any";
		protected override bool AreEqual(TypeSpecifier obj) => obj is AnyTypeSpecifier;
		public override object Clone() => new AnyTypeSpecifier();
	}

	public class PrimitiveTypeSpecifier(NBTType type) : TypeSpecifier
	{
		public override bool Operable => NBTValue.IsOperableType(Type);
		public override bool IsList => Type == NBTType.List || Type == NBTType.IntArray || Type == NBTType.LongArray || Type == NBTType.ByteArray;
		public readonly NBTType Type = type;
		public override NBTType EffectiveType => Type;

		//public override bool IsAssignableTo(TypeSpecifier other) => (other.IsList && Type == NBTType.List) || base.IsAssignableTo(other);
		public override TypeSpecifier? Property(string name) => Type == NBTType.Compound ? new AnyTypeSpecifier() : base.Property(name);

		protected override bool AreEqual(TypeSpecifier obj) => obj is PrimitiveTypeSpecifier p && p.Type == Type;
		public override string ToString() => Enum.GetName(Type)?.ToLower() ?? throw new InvalidOperationException();
		public override object Clone() => new PrimitiveTypeSpecifier(Type);

		public override LiteralValue DefaultValue => Type switch
		{
			NBTType.Boolean => new(false),
			NBTType.Byte => new(new NBTByte(0)),
			NBTType.Short => new(new NBTShort(0)),
			NBTType.Int => new(new NBTInt(0)),
			NBTType.Long => new(new NBTLong(0)),
			NBTType.Float => new(new NBTFloat(0)),
			NBTType.Double => new(new NBTDouble(0)),
			NBTType.List => new(new NBTList()),
			NBTType.Compound => new(new NBTCompound()),
			_ => throw new NotImplementedException()
		};

		public static PrimitiveTypeSpecifier Int => new(NBTType.Int);
		public static PrimitiveTypeSpecifier Bool => new(NBTType.Boolean);
		public static PrimitiveTypeSpecifier String => new(NBTType.String);
		public static PrimitiveTypeSpecifier Compound => new(NBTType.Compound);
		public static PrimitiveTypeSpecifier List => new(NBTType.List);
	}

	public class FunctionTypeSpecifier(FunctionModifiers modifiers, TypeSpecifier returnType, IEnumerable<Parameter> paramters) : TypeSpecifier
	{
		public readonly FunctionModifiers Modifiers = modifiers;
		public readonly TypeSpecifier ReturnType = returnType;
		public readonly Parameter[] Parameters = [.. paramters];
		public override bool Operable => false;
		public override IEnumerable<TypeSpecifier> Subtypes => [ReturnType, .. Parameters.Select(i => i.Type)];

		public FunctionTypeSpecifier ApplyGenericWithParams(TypeSpecifier[] args)
		{
			var newArgs = new Parameter[Parameters.Length];

			for (int i = 0; i < Parameters.Length; i++)
			{
				if (args.Length > i) newArgs[i] = new(Parameters[i].Modifiers, Parameters[i].Type.ApplyGeneric(args[i]), Parameters[i].Name);
				else newArgs[i] = Parameters[i];
			}

			var other = new FunctionTypeSpecifier(Modifiers, ReturnType, newArgs);
			return (FunctionTypeSpecifier)ApplyGeneric(other);
		}

		protected override bool AreEqual(TypeSpecifier obj) => obj is FunctionTypeSpecifier f
			&& f.Modifiers == Modifiers
			&& f.ReturnType == ReturnType
			&& Parameters.Length == f.Parameters.Length
			&& Parameters.Zip(f.Parameters).All(i => i.First == i.Second);

		// TODO: properly do this
		public override string ToString() => $"{ReturnType}({string.Join(", ", Parameters.Select(p => $"{p.Type} {p.Name}"))})";
		public string ToString(string name) => $"{ReturnType} {name}({string.Join(", ", Parameters.Select(p => $"{p.Type} {p.Name}"))})";

		public override object Clone() => new FunctionTypeSpecifier(Modifiers, (TypeSpecifier)ReturnType.Clone(), Parameters.Select(i => new Parameter(i.Modifiers, (TypeSpecifier)i.Type.Clone(), i.Name)));

		public static FunctionTypeSpecifier VoidFunc => new(FunctionModifiers.None, new VoidTypeSpecifier(), []);

		public override LiteralValue DefaultValue => throw new InvalidTypeError(ToString());
	}

	public class DynamicFunctionTypeSpecifier(TypeSpecifier returnType) : FunctionTypeSpecifier(FunctionModifiers.None, returnType, [])
	{
		protected override bool AreEqual(TypeSpecifier obj) => base.AreEqual(obj) && obj is DynamicFunctionTypeSpecifier;
	}

	public class ListTypeSpecifier(TypeSpecifier inner) : TypeSpecifier
	{
		public readonly TypeSpecifier Inner = inner;
		public override bool Operable => false;
		public override NBTType EffectiveType => NBTType.List;
		public override IEnumerable<TypeSpecifier> Subtypes => [Inner];
		public override bool IsList => true;
		public override string BasePath => Inner.BasePath;
		public override TypeSpecifier BaseClass => PrimitiveTypeSpecifier.List;

		public override LiteralValue DefaultValue => new(new NBTList(), this);

		// public override bool IsAssignableTo(TypeSpecifier other) => other.EffectiveType == NBTType.List || base.IsAssignableTo(other);
		protected override bool AreEqual(TypeSpecifier obj) => obj is ListTypeSpecifier arr && arr.Inner == Inner;
		public override string ToString() => $"{Inner}[]";
		public override object Clone() => new ListTypeSpecifier((TypeSpecifier)Inner.Clone());
	}

	public class GenericTypeSpecifier(string name, TypeSpecifier? constraint = null, bool resolved = false) : TypeSpecifier
	{
		public readonly string Name = name;
		public TypeSpecifier Constraint { get; private set; } = constraint ?? PrimitiveTypeSpecifier.Compound;
		public bool Resolved { get; private set; } = resolved;

		public override bool Operable => Constraint.Operable;
		public override LiteralValue DefaultValue => Constraint.DefaultValue;
		public override bool IsList => Constraint.IsList;
		public override string BasePath => Constraint.BasePath;
		public override TypeSpecifier BaseClass => Constraint;
		public override NBTType EffectiveType => Constraint.EffectiveType;
		public override IEnumerable<TypeSpecifier> Subtypes => Constraint.Subtypes;

		// Also kinda does the GetHashCode as well because I did a silly for that
		public override string ToString() => Resolved ? Constraint.ToString() : Name;

		public override bool ConstraintSatisfiedBy(TypeSpecifier other) => other.Implements(Constraint);
		public override bool Implements(TypeSpecifier other)
		{
			if (this == other) return true;
			else if (Constraint == other) return true;
			else return BaseClass.Implements(other);
		}

		public void Set(TypeSpecifier type)
		{
			Constraint = type;
			Resolved = true;
		}

		protected override void ApplyGeneric(TypeSpecifier other, Dictionary<string, TypeSpecifier> typeMap)
		{
			if (!Resolved)
			{
				if (typeMap.TryGetValue(Name, out var type)) other = type;
				Set(other);
				typeMap[Name] = other;
			}
			else if (other != typeMap[Name]) throw new NotImplementedException(); // Maybe remove this

			base.ApplyGeneric(other, typeMap);
		}

		protected override bool AreEqual(TypeSpecifier obj) => obj is GenericTypeSpecifier other && other.Name == Name && other.Constraint == Constraint;
		public override TypeSpecifier GetEquatableType() => Resolved ? Constraint.GetEquatableType() : this;
		public override int GetHashCode() => HashCode.Combine(Name, Constraint);
		public override object Clone() => new GenericTypeSpecifier(Name, Constraint, Resolved);
	}
}
