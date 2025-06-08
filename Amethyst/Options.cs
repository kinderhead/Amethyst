using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst
{
    public class Options
    {
		[Option('o', "output",  HelpText = "Zipped datapack, defaults to first input file's name.")]
		public string Output { get; set; }

		[Option('f', "pack-format", HelpText = "Datapack format.", Default = 71)]
		public int PackFormat { get; set; }

		[Option('a', "allow-one-liners", HelpText = "Allow one command subfunctions instead of optimizing them out.")]
		public bool AllowOneLiners { get; set; }

		[Option('c', "no-constant-scores", HelpText = "Don't generate global number constant scores to use when applicable.")]
		public bool NoConstantScores { get; set; }

		[Option('k', "keep-locals-on-stack", HelpText = "By default, ints are stored as scores.")]
		public bool KeepLocalsOnStack { get; set; }

		[Value(0, MetaName = "input files", HelpText = "Files to compile.", Required = true)]
        public IEnumerable<string> Inputs { get; set; }
    }
}
