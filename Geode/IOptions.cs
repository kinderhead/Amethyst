namespace Geode
{
	public interface IOptions
	{
		string Output { get; set; }
		bool DumpIR { get; set; }
		bool Debug { get; set; }
	}
}
