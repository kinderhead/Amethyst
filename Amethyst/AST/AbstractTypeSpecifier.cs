using System.Diagnostics;
using System.Numerics;
using Amethyst.Errors;
using Amethyst.IR.Types;
using Datapack.Net.Utils;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST
{
    public abstract class AbstractTypeSpecifier(LocationRange loc)
        : Node(loc), IEquatable<AbstractTypeSpecifier>, IEqualityOperators<AbstractTypeSpecifier, AbstractTypeSpecifier, bool>
    {
        public static bool operator ==(AbstractTypeSpecifier? left, AbstractTypeSpecifier? right) => left?.Equals(right) ?? false;
        public static bool operator !=(AbstractTypeSpecifier? left, AbstractTypeSpecifier? right) => !left?.Equals(right) ?? true;
        public abstract bool Equals(AbstractTypeSpecifier? other);

        public TypeSpecifier Resolve(FunctionContext ctx, bool allowAuto = false) => Resolve((Compiler)ctx.Compiler, ctx.Decl.ID.GetContainingFolder(), allowAuto);

        public TypeSpecifier Resolve(Compiler ctx, string baseNamespace, bool allowAuto = false)
        {
            TypeSpecifier? ret = null;
            return !ctx.WrapError(Location, [DebuggerNonUserCode]() => ret = ResolveImpl(ctx, baseNamespace, allowAuto)) ? throw new EmptyGeodeError() : ret!;
        }

        protected abstract TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false);
        public abstract IEnumerable<NamespacedID> SoftResolve(Compiler ctx, string baseNamespace, bool allowAuto = false);

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is AbstractTypeSpecifier t && Equals(t);
        }

        public abstract override int GetHashCode();
    }

    public class SimpleAbstractTypeSpecifier(LocationRange loc, string type) : AbstractTypeSpecifier(loc)
    {
        public readonly string Type = type;

        public override bool Equals(AbstractTypeSpecifier? other) => other is SimpleAbstractTypeSpecifier t && t.Type == Type;

        protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false)
        {
            return Type switch
            {
                "var" => allowAuto ? new VarType() : throw new InvalidTypeError(Type),
                _ => ctx.TypeHandler.Find(baseNamespace, Type)
            };
        }

        public override IEnumerable<NamespacedID> SoftResolve(Compiler ctx, string baseNamespace, bool allowAuto = false)
        {
            return Type switch
            {
                "var" => allowAuto ? ["amethyst:var"] : throw new InvalidTypeError(Type),
                _ => [ctx.TypeHandler.FindSoft(baseNamespace, Type)]
            };
        }

        public override int GetHashCode() => HashCode.Combine(Type, GetType());
    }

    public class AbstractListTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
    {
        public readonly AbstractTypeSpecifier Inner = inner;

        public override bool Equals(AbstractTypeSpecifier? other) => other is AbstractListTypeSpecifier t && t.Inner == Inner;
        public override int GetHashCode() => HashCode.Combine(Inner, GetType());

        protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false) => new ListType(Inner.Resolve(ctx, baseNamespace));
        public override IEnumerable<NamespacedID> SoftResolve(Compiler ctx, string baseNamespace, bool allowAuto = false) => Inner.SoftResolve(ctx, baseNamespace, allowAuto);
    }

    public class AbstractMapTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
    {
        public readonly AbstractTypeSpecifier Inner = inner;

        public override bool Equals(AbstractTypeSpecifier? other) => other is AbstractMapTypeSpecifier t && t.Inner == Inner;
        public override int GetHashCode() => HashCode.Combine(Inner, GetType());

        protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false)
        {
            var inner = Inner.Resolve(ctx, baseNamespace);
            return inner is ReferenceType and not WeakReferenceType ? throw new ReferenceMapError() : new SimpleMapType(inner);
        }

        public override IEnumerable<NamespacedID> SoftResolve(Compiler ctx, string baseNamespace, bool allowAuto = false) => Inner.SoftResolve(ctx, baseNamespace, allowAuto);
    }

    public class AbstractReferenceTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
    {
        public readonly AbstractTypeSpecifier Inner = inner;

        public override bool Equals(AbstractTypeSpecifier? other) => other is AbstractReferenceTypeSpecifier t && t.Inner == Inner;
        public override int GetHashCode() => HashCode.Combine(Inner, GetType());

        protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false) => new ReferenceType(Inner.Resolve(ctx, baseNamespace));
        public override IEnumerable<NamespacedID> SoftResolve(Compiler ctx, string baseNamespace, bool allowAuto = false) => Inner.SoftResolve(ctx, baseNamespace, allowAuto);
    }

    public class AbstractWeakReferenceTypeSpecifier(LocationRange loc, AbstractTypeSpecifier inner) : AbstractTypeSpecifier(loc)
    {
        public readonly AbstractTypeSpecifier Inner = inner;

        public override bool Equals(AbstractTypeSpecifier? other) => other is AbstractWeakReferenceTypeSpecifier t && t.Inner == Inner;
        public override int GetHashCode() => HashCode.Combine(Inner, GetType());

        protected override TypeSpecifier ResolveImpl(Compiler ctx, string baseNamespace, bool allowAuto = false) => new WeakReferenceType(Inner.Resolve(ctx, baseNamespace));
        public override IEnumerable<NamespacedID> SoftResolve(Compiler ctx, string baseNamespace, bool allowAuto = false) => Inner.SoftResolve(ctx, baseNamespace, allowAuto);
    }
}