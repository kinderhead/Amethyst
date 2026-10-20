using Geode;

namespace Amethyst
{
    public interface IAmethystOptions : IOptions
    {
        bool DumpCommands { get; set; }
        string[] Inputs { get; set; }
        string[] Include { get; set; }
        string[] Link { get; set; }
    }
}