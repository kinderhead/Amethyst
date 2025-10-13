using CommandLine;
using Geode;

namespace Amethyst
{
	public class Options : IOptions
	{
		[Option('o', "output", HelpText = "Zipped datapack, defaults to first input file's name.")]
		public required string Output { get; set; }

		[Option('d', "debug", HelpText = "Enable debug checks.", Default = false)]
		public bool Debug { get; set; }


		[Option("dump-ir", HelpText = "Dump Geode IR and don't compile to datapack.")]
		public bool DumpIR { get; set; }

		[Value(0, MetaName = "input files", HelpText = "Files to compile.", Required = true)]
		public required IEnumerable<string> Inputs { get; set; }
	}
}
