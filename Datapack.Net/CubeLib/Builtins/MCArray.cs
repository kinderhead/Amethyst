using Datapack.Net.CubeLib.Utils;
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
    public partial class MCList<T>(IPointer<MCList<T>> loc) : RuntimeObject<CubeLibStd, MCList<T>>(loc)
    {
        public void Add(T value)
        {
            if (NBTType.IsNBTType<T>()) _Add([new("value", value?.ToString())]);
            else _Add([new("value", value)]);
        }

        public void ForEach(Action<IPointer<T>, ScoreRef> loop)
        {
            var proj = Project.ActiveProject;

            var count = Count();
            proj.For(0, count, (idex) =>
            {
                var i = this[idex];
                loop(i, idex);
            });
        }

        public void Count(ScoreRef loc) => Pointer.Dereference(loc);
        public ScoreRef Count()
        {
            var reg = Project.ActiveProject.Local();
            Count(reg);
            return reg;
        }

        public RuntimePointer<T> Index(object index)
        {
            var proj = Project.ActiveProject;

            var ptr = proj.AllocObj<RuntimePointer<T>>(false);
            proj.Std.PointerIndexList([.. ptr.Obj.Pointer.StandardMacros([], "1"), .. Pointer.StandardMacros([], "2"), new("index", index)]);
            proj.WithCleanup(ptr.FreeObj);
            return ptr;
        }

        public IPointer<T> this[int index]
        {
            get => GetProp<T>($"[{index}]", false);
            set => value.Copy(GetProp<T>($"[{index}]", false));
        }

        public IPointer<T> this[ScoreRef index]
        {
            get => Index(index);
            set => value.Copy(Index(index));
        }

        public IPointer<T> this[IPointer<NBTInt> index]
        {
            get => Index(index);
            set => value.Copy(Index(index));
        }

        [DeclareMC("init")]
        private static void _Init(MCList<T> self)
        {
            self.Pointer.Set(new NBTList());
        }

        /// <summary>
        /// Arguments: <br/>
        /// <b>value</b>: The value to add <br/>
        /// </summary>
        /// <param name="self">Self</param>
        [DeclareMC("add", ["value"])]
        private static void __Add(MCList<T> self)
        {
            State.Std.PointerAppend(self.Pointer.StandardMacros([new("value", new NBTRawString("$(value)"))]), true);
        }
    }
}
