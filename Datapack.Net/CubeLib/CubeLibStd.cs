using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public class CubeLibStd(DP pack) : Project(pack)
    {
        public override string Namespace => "cubelib";

        [DeclareMC("test")]
        public void Test()
        {
            Print("Test");
        }

        /// <summary>
        /// Arguments: <br/>
        /// <b>storage</b>: Storage identifier <br/>
        /// <b>path</b>: Path in storage to heap
        /// </summary>
        [DeclareMC("alloc_address", true, ["storage", "path"])]
        public void AllocAddress()
        {
            var x = Local();
            WhileTrue(() =>
            {
                Random(new(0, int.MaxValue - 1), x);

                var res = Local();
                CallRet(StorageExistsConcat, res, [new("storage", "$(storage)"), new("path1", "$(path)"), new("path2", x)], true);
                If(res == 1, new ReturnCommand(1));
            });
            Return(x);
        }

        /// <summary>
        /// Arguments: <br/>
        /// <b>storage</b>: Storage identifier <br/>
        /// <b>path1</b>: Path in storage to check
        /// <b>path2</b>: Concatenates with path1
        /// </summary>
        [DeclareMC("storage_exists_concat", true, ["storage", "path1", "path2"])]
        public void StorageExistsConcat()
        {
            AddCommand(new Execute(true).If.Data(new StorageMacro("$(storage)"), "$(path1).$(path2)").Run(new ReturnCommand(0)));
            ReturnFail();
        }
    }
}
