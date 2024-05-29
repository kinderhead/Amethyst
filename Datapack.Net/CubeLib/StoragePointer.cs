using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public class StoragePointer(MCHeap heap, ScoreRef key)
    {
        public readonly MCHeap Heap = heap;
        public readonly ScoreRef Key = key;
    }
}
