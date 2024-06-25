using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib.Builtins.Static
{
    public class MCStaticHeap(Storage storage, string path) : IStaticType
    {
        public readonly Storage Storage = storage;
        public readonly string Path = path;

        public void Init()
        {
            Project.ActiveProject.PrependMain(new Execute().Unless.Data(Storage, Path).Run(new DataCommand.Modify(Storage, Path).Set().Value("{}")));
        }

        public HeapPointer<T> Alloc<T>(ScoreRef loc) where T : IPointerable
        {
            var p = Project.ActiveProject;
            p.Std.AllocAddress([new("storage", Storage.ID), new("path", Path)], loc);
            var pointer = new HeapPointer<T>(this, loc);
            return pointer;
        }

        public RuntimePointer<T> Alloc<T>(RuntimePointer<T> loc) where T : IPointerable
        {
            var p = Project.ActiveProject;
            var ret = p.Temp(0, "alloc");
            p.Std.AllocAddress([new("storage", Storage.ID), new("path", Path)], ret);
            p.Std.PointerSet(loc.Obj.Pointer.StandardMacros([new("value", ret)]));
            return loc;
        }

        public void Clear()
        {
            Project.ActiveProject.AddCommand(new DataCommand.Modify(Storage, Path).Set().Value("{}"));
        }
    }
}
