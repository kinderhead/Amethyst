using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib.Builtins
{
    [RuntimeObject("list")]
    public partial class MCList(IPointer<MCList> loc) : RuntimeObject<CubeLibStd, MCList>(loc)
    {
        internal sealed class Props
        {
            [RuntimeProperty("value")]
            public NBTList List { get; set; }

            [RuntimeProperty("count")]
            public int Count { get; set; }
        }

        public void Add(IRuntimeArgument value) => Add([new("value", value)]);
        public void Add(NBTType value) => Add([new("value", value.ToString())]);

        [DeclareMC("init")]
        private static void _Init(MCList self)
        {
            self.List = new NBTList();
            self.Count = 0;
        }

        /// <summary>
        /// Arguments: <br/>
        /// <b>value</b>: The value to add <br/>
        /// </summary>
        /// <param name="self">Self</param>
        [DeclareMC("add", ["value"])]
        private static void _Add(MCList self)
        {
            State.Std.PointerAppend(self.List.Pointer.StandardMacros([new("value", "$(value)")]), true);
        }
    }
}
