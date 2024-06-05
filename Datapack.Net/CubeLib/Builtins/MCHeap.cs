using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib.Builtins
{
    public class MCHeap(Storage storage, string path) : IStaticType
    {
        public readonly Storage Storage = storage;
        public readonly string Path = path;

        public void Init()
        {
            Project.ActiveProject.PrependMain(new Execute().Unless.Data(Storage, Path).Run(new DataCommand.Modify(Storage, Path).Set().Value("{}")));
        }

        public HeapPointer<T> Alloc<T>(ScoreRef loc)
        {
            var p = Project.ActiveProject;
            p.Std.AllocAddress([new("storage", Storage.ID), new("path", Path)], loc);
            var pointer = new HeapPointer<T>(this, loc);
            return pointer;
        }

        public void Clear()
        {
            Project.ActiveProject.AddCommand(new DataCommand.Modify(Storage, Path).Set().Value("{}"));
        }
    }
}
