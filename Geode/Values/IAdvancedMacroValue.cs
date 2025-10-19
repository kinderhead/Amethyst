namespace Geode.Values
{
	public interface IAdvancedMacroValue : IValue
	{
		IConstantValue Macroize(Func<IValue, IConstantValue> apply);
	}
}
