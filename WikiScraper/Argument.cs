using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiScraper
{
	public abstract class Property
	{
		public abstract string Type { get; }

		public static bool operator ==(Property a, Property b) => a.Type == b.Type;
		public static bool operator !=(Property a, Property b) => a.Type != b.Type;

		public override bool Equals(object? obj)
		{
			if (obj is Property prop) return this == prop;
			return false;
		}

		public override int GetHashCode() => Type.GetHashCode();
	}

	public class StringProperty : Property
	{
		public override string Type => "string";
	}

	public class ListProperty : Property
	{
		public override string Type => "NBTList";
	}

	public class CompoundProperty : Property
	{
		public override string Type => "NBTCompound";
	}

	public class IntArrayProperty : Property
	{
		public override string Type => "NBTIntArray";
	}

	public readonly record struct Definition(Property Type, string Name);
}
