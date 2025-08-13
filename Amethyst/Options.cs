using CommandLine;

namespace Amethyst
{
	public class Options
	{
		[Option('o', "output", HelpText = "Zipped datapack, defaults to first input file's name.")]
		public string Output { get; set; }

		[Option('f', "pack-format", HelpText = "Datapack format.", Default = 71)]
		public int PackFormat { get; set; }

		[Option('n', "no-constant-scores", HelpText = "Don't generate global number constant scores to use when applicable.")]
		public bool NoConstantScores { get; set; }

		[Option("dump-ir", HelpText = "Dump Geode IR and don't compile to datapack.")]
		public bool DumpIR { get; set; }

		[Value(0, MetaName = "input files", HelpText = "Files to compile.", Required = true)]
		public IEnumerable<string> Inputs { get; set; }
	}
}
