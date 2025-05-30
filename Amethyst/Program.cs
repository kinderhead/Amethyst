using CommandLine;

namespace Amethyst
{
	internal class Program
	{
		static void Main(string[] args)
		{
			if (!new Compiler(args).Compile()) Environment.Exit(1);
		}
	}
}
