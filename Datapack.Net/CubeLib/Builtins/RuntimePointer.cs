using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib.Builtins
{
    [RuntimeObject("pointer")]
    public partial class RuntimePointer<T>(IPointer<RuntimePointer<T>> loc, string extraPath) : RuntimeObject<CubeLibStd, RuntimePointer<T>>(loc), IPointer<T> where T : Pointerable
    {
        public readonly string ExtraPath = extraPath;

        private bool selfPointer = false;
        public RuntimePointer<T> SelfPointer
        {
            get
            {
                var ptr = new RuntimePointer<T>(Pointer, ExtraPath)
                {
                    selfPointer = true
                };
                return ptr;
            }
        }

        public T Self => (T?)typeof(T).GetMethod("Create")?.Invoke(null, [this]) ?? throw new ArgumentException("Not a pointer to a RuntimeObject");

        public RuntimePointer(IPointer<RuntimePointer<T>> loc) : this(loc, "")
        {

        }

        public void Set(NBTType val)
        {
            Project.ActiveProject.Std.PointerSet(StandardMacros([new("value", val.ToString())]));
        }

        public void Copy(IPointer<T> dest) => CopyUnsafe(dest);

        public void CopyUnsafe(IStandardPointerMacros dest)
        {
            Project.ActiveProject.Std.PointerMove([.. StandardMacros(null, "2"), .. dest.StandardMacros(null, "1")]);
        }

        public void Move(IPointer<T> dest)
        {
            Copy(dest);
            Free();
        }

        public void MoveUnsafe(IStandardPointerMacros dest)
        {
            CopyUnsafe(dest);
            Free();
        }

        public void Dereference(ScoreRef val) => Project.ActiveProject.Std.PointerDereferenceToScore(StandardMacros(), val);
        public ScoreRef Dereference()
        {
            var ret = Project.ActiveProject.Local();
            Dereference(ret);
            return ret;
        }

        public KeyValuePair<string, object>[] StandardMacros(KeyValuePair<string, object>[]? extras = null, string postfix = "")
        {
            extras ??= [];

            return [new($"storage{postfix}", Project.ActiveProject.Heap.Storage),
                new($"path{postfix}", Project.ActiveProject.Heap.Path),
                new($"pointer{postfix}", Obj),
                new($"ext{postfix}", ExtraPath),
                .. extras];
        }

        public override IPointer ToPointer() => selfPointer ? Pointer : this;

        public IPointer<R> Get<R>(string path, bool dot) where R : Pointerable => new RuntimePointer<R>(Pointer.Cast<RuntimePointer<R>>(), ExtraPath + (dot ? "." : "") + path);

        public void Resolve(IPointer<RuntimePointer<T>> dest)
        {
            Pointer.Copy(dest);
            throw new NotImplementedException();
        }

        public void Free()
        {
            Project.ActiveProject.Std.PointerFree(StandardMacros());
            FreeObj();
        }

        public IPointer<R> Cast<R>() where R : Pointerable => new RuntimePointer<R>(Pointer.Cast<RuntimePointer<R>>());

        public PointerExists Exists() => new() { Pointer = this };

        public BaseHeapPointer GetHeapPointer() => Pointer.GetHeapPointer();

        public IPointer<T> Local()
        {
            Project.ActiveProject.WithCleanup(Free);
            return this;
        }

        internal sealed class Props
        {
            [RuntimeProperty("obj")]
            public NBTString Obj { get; set; }
        }
    }
}
