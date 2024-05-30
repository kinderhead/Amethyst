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

        public HeapPointer Alloc(ScoreRef loc)
        {
            var p = Project.ActiveProject;
            p.CallRet(p.Std.AllocAddress, loc, [new("storage", Storage.ID), new("path", Path)]);
            var pointer = new HeapPointer(this, loc);
            return pointer;
        }

        public void Clear()
        {
            Project.ActiveProject.AddCommand(new DataCommand.Modify(Storage, Path).Set().Value("{}"));
        }
    }
}
