using Geode;
using System;

namespace Amethyst
{
    public interface IAmethystOptions : IOptions
    {
        public bool DumpCommands { get; set; }
        public string[] Inputs { get; set; }
    }
}
