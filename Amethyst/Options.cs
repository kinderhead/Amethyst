using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst
{
    public class Options
    {
		[Option('o', "output",  HelpText = "Zipped datapack, defaults to first input file's name", Required = false)]
		public string Output { get; set; }

		[Option("pack-format", HelpText = "Datapack format", Required = false, Default = 71)]
		public int PackFormat { get; set; }

		[Value(0, MetaName = "input files", HelpText = "Files to compile", Required = true)]
        public IEnumerable<string> Inputs { get; set; }
    }
}
