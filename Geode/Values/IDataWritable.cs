namespace Geode.Values
{
	public interface IDataWritable : IValue
	{
		void StoreTo(DataTargetValue val, RenderContext ctx);
	}
}
