using Datapack.Net.Pack;

namespace Geode
{
    public interface IOptions
    {
        string Output { get; set; }
        string? Data { get; set; }
        PackFormat PackFormat { get; set; }
        bool DumpIR { get; set; }
        bool Debug { get; set; }
        int OptimizationLevel { get; set; }
    }
}