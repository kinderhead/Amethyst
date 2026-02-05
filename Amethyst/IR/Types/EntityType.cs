using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.IR.Types
{
	public class EntityType(NamespacedID id, TypeSpecifier? baseClass, Dictionary<string, TypeSpecifier> props, Dictionary<string, FunctionType> methods) : StructType(id, baseClass, props, methods)
	{
		public override LiteralValue DefaultValue => new(0, this);
		public override NBTType EffectiveType => NBTType.Int;

		public override ValueRef? GetPropertyOverload(ValueRef val, string name, FunctionContext ctx) => new VoidValue();

		public override object Clone() => new EntityType(ID, BaseClass, Properties, Methods);
		public override string ToString() => ID.ToString();
		protected override bool EqualsImpl(TypeSpecifier obj) => obj is EntityType other && other.ID == ID;

		public static readonly EntityType Dummy = new("amethyst:dummy", null, [], []);
	}
}
