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
    [RuntimeObject("list", false)]
    public partial class MCList<T>(IPointer<MCList<T>> loc) : RuntimeObject<CubeLibStd, MCList<T>>(loc) where T : IBaseRuntimeObject
    {
        internal sealed class Props
        {
            [RuntimeProperty("list")]
            public NBTList List { get; set; }
        }

        public void Add(T value)
        {
            Project.ActiveProject.Std.PointerAppend(List.Pointer.StandardMacros([new("value", new NBTCompound())]), true);
            value.GetPointer().ToRTP(this[-1].GetPointer().ToRTP<T>());
            if (Project.Settings.ReferenceChecking) value.ReferenceCount.Pointer.With(i => i.Add(1));
        }

        public void Remove(int index) => this[index].GetPointer().Free();
        public void Remove(ScoreRef index) => this[index].GetPointer().Free();
        public void Remove(IPointer<NBTInt> index) => this[index].GetPointer().Free();

        public void Clear() => Pointer.Set(new NBTList());

        public void ForEach(Action<T, ScoreRef> loop)
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

        public RuntimePointer<RuntimePointer<T>> Index(object index)
        {
            var proj = Project.ActiveProject;

            var ptr = proj.AllocObj<RuntimePointer<RuntimePointer<T>>>(false);
            proj.Std.PointerIndexList([.. ptr.Obj.Pointer.StandardMacros([], "1"), .. List.Pointer.StandardMacros([], "2"), new("index", index)]);
            proj.WithCleanup(ptr.FreeObj);
            return ptr;
        }

        public T this[int index]
        {
            get => T.Create(new RuntimePointer<T>(List.Pointer.Get<RuntimePointer<T>>($"[{index}]", false)));
            set => value.GetPointer().CopyUnsafe(List.Pointer.Get<T>($"[{index}]", false));
        }

        public T this[ScoreRef index]
        {
            get => T.Create(new RuntimePointer<T>(Index(index)));
            set => value.GetPointer().CopyUnsafe(Index(index));
        }

        public T this[IPointer<NBTInt> index]
        {
            get => T.Create(new RuntimePointer<T>(Index(index)));
            set => value.GetPointer().CopyUnsafe(Index(index));
        }

        public override void Destruct()
        {
            ForEach((i, idex) =>
            {
                if (i.GetPointer() is RuntimePointer<T> ptr)
                {
                    ptr.RemoveOneReference();
                }
                else throw new Exception();
            });
        }

        [DeclareMC("init")]
#pragma warning disable IDE1006
		private static void _Init(MCList<T> self)
#pragma warning restore IDE1006
		{
            self.List = new NBTList();
        }
    }
}
