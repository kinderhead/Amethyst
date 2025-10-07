namespace Geode
{
	public interface IOptions
	{
		string Output { get; set; }
		int PackFormat { get; set; }
		bool DumpIR { get; set; }
		bool Debug { get; set; }
	}
}
