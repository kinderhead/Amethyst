using Geode;

namespace Amethyst
{
	public interface IAmethystOptions : IOptions
	{
		bool DumpCommands { get; set; }
		string[] Inputs { get; set; }
	}
}