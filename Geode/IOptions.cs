namespace Geode
{
	public interface IOptions
	{
		string Output { get; set; }
		string PackFormat { get; set; }
		bool DumpIR { get; set; }
		bool Debug { get; set; }
	}
}
