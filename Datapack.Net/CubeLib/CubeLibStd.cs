using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    [Project]
    public partial class CubeLibStd(DP pack) : Project(pack)
    {
        public override string Namespace => "cubelib";

        [DeclareMC("test")]
        private void _Test()
        {
            Print("Test");
        }

        /// <summary>
        /// Arguments: <br/>
        /// <b>storage</b>: Storage identifier <br/>
        /// <b>path</b>: Path in storage to heap
        /// </summary>
        [DeclareMCReturn<ScoreRef>("alloc_address", ["storage", "path"])]
        private void _AllocAddress()
        {
            var x = Local();
            WhileTrue(() =>
            {
                Random(new(0, int.MaxValue - 1), x);

                var res = Local();
                PointerExists([new("storage", "$(storage)"), new("path", "$(path)"), new("pointer", x)], res, true);
                If(res == 0, new ReturnCommand(1));
            });
            Return(x);
        }

        /// <summary>
        /// Arguments: <br/>
        /// <b>storage</b>: Storage identifier <br/>
        /// <b>path</b>: Path in storage to heap <br/>
        /// <b>pointer</b>: The pointer <br/>
        /// <b>ext</b>: Extension path including "." <br/>
        /// </summary>
        [DeclareMCReturn<ScoreRef>("pointer_exists", ["storage", "path", "pointer", "ext"])]
        private void _PointerExists()
        {
            AddCommand(new Execute(true).If.Data(new StorageMacro("$(storage)"), "$(path).$(pointer)$(ext)").Run(new ReturnCommand(1)));
            ReturnFail();
        }

        /// <summary>
        /// Arguments: <br/>
        /// <b>storage</b>: Storage identifier <br/>
        /// <b>path</b>: Path in storage to heap <br/>
        /// <b>pointer</b>: The pointer <br/>
        /// <b>ext</b>: Extension path including "." <br/>
        /// <b>value</b>: Value
        /// </summary>
        [DeclareMC("pointer_set", ["storage", "path", "pointer", "ext", "value"])]
        private void _PointerSet()
        {
            AddCommand(new DataCommand.Modify(new StorageMacro("$(storage)"), "$(path).$(pointer)$(ext)", true).Set().Value("$(value)"));
        }

        /// <summary>
        /// Arguments: <br/>
        /// <b>storage</b>: Storage identifier <br/>
        /// <b>path</b>: Path in storage to heap <br/>
        /// <b>pointer</b>: The pointer <br/>
        /// <b>ext</b>: Extension path including "." <br/>
        /// </summary>
        [DeclareMCReturn<ScoreRef>("pointer_dereference", ["storage", "path", "pointer", "ext"])]
        private void _PointerDereference()
        {
            var x = Local();
            AddCommand(x.Store(true).Run(new DataCommand.Get(new StorageMacro("$(storage)"), "$(path).$(pointer)$(ext)")));
            Return(x);
        }

        /// <summary>
        /// Arguments: <br/>
        /// <b>storage</b>: Storage identifier <br/>
        /// <b>path</b>: Path in storage to heap <br/>
        /// <b>pointer</b>: The pointer <br/>
        /// <b>ext</b>: Extension path including "." <br/>
        /// </summary>
        [DeclareMC("pointer_free", ["storage", "path", "pointer", "ext"])]
        private void _PointerFree()
        {
            AddCommand(new DataCommand.Remove(new StorageMacro("$(storage)"), "$(path).$(pointer)$(ext)", true));
        }

        /// <summary>
        /// Arguments: <br/>
        /// <b>storage</b>: Storage identifier <br/>
        /// <b>path</b>: Path in storage to heap <br/>
        /// <b>pointer</b>: The pointer <br/>
        /// <b>ext</b>: Extension path including "." <br/>
        /// </summary>
        [DeclareMC("pointer_print", ["storage", "path", "pointer", "ext"])]
        private void _PointerPrint()
        {
            AddCommand(new TellrawCommand(new TargetSelector(TargetType.a), new FormattedText().Storage(new StorageMacro("$(storage)"), "$(path).$(pointer)$(ext)"), true));
        }

        /// <summary>
        /// Arguments: <br/>
        /// <b>storage1</b>: Destination storage identifier <br/>
        /// <b>path1</b>: Destination path in storage to heap <br/>
        /// <b>pointer1</b>: Destination pointer <br/>
        /// <b>ext1</b>: Destination extension path including "." <br/>
        /// <b>storage2</b>: Source storage identifier <br/>
        /// <b>path2</b>: Source path in storage to heap <br/>
        /// <b>pointer2</b>: Source pointer <br/>
        /// <b>ext2</b>: Source extension path including "." <br/>
        /// </summary>
        [DeclareMC("pointer_move", ["storage1", "path1", "pointer1", "ext1", "storage2", "path2", "pointer2", "ext2"])]
        private void _PointerMove()
        {
            AddCommand(new DataCommand.Modify(new StorageMacro("$(storage1)"), "$(path1).$(pointer1)$(ext1)", true).Set().From(new StorageMacro("$(storage2)"), "$(path2).$(pointer2)$(ext2)"));
        }
    }
}
