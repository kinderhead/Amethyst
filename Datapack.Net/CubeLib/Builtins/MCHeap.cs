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

        public StoragePointer Alloc()
        {
            var p = Project.ActiveProject;

            throw new NotImplementedException();
        }
    }
}
