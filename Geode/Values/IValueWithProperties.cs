namespace Geode.Values
{
	public interface IValueWithProperties<T> : IValue where T : IValue
	{
		T Property(string prop, TypeSpecifier type);
	}
}
