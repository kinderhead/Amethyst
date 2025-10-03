namespace Amethyst.Geode.Values
{
	public interface IValueWithProperties<T> where T : Value
	{
		T Property(string prop, TypeSpecifier type);
	}
}
