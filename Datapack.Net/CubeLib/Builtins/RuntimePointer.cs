using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib.Builtins
{
    [RuntimeObject("pointer", false)]
    public partial class RuntimePointer<T>(IPointer<RuntimePointer<T>> loc, string extraPath) : RuntimeObject<CubeLibStd, RuntimePointer<T>>(loc), IPointer<T> where T : IPointerable
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

        public T Self => (T?)Activator.CreateInstance(typeof(T), this) ?? throw new ArgumentException("Not a pointer to a RuntimeObject");
        public bool IsRuntimeObject => typeof(T).IsAssignableTo(typeof(IBaseRuntimeObject)) && !(typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(RuntimePointer<>));

        public RuntimePointer(IPointer<RuntimePointer<T>> loc) : this(loc, "")
        {

        }

        public void Set(NBTValue val)
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

        public IPointer<R> Get<R>(string path, bool dot = true) where R : IPointerable => new RuntimePointer<R>(Pointer.Cast<RuntimePointer<R>>(), ExtraPath + (dot ? "." : "") + path);

        public void Resolve(IPointer<RuntimePointer<T>> dest)
        {
            Pointer.Copy(dest);
            throw new NotImplementedException();
        }

        public void Free()
        {
            if (Project.Settings.ReferenceChecking && IsRuntimeObject && Self is IBaseRuntimeObject obj) obj.Destruct();
            Project.ActiveProject.Std.PointerFree(StandardMacros());
            Freed = true;
            FreeObj();
        }

        public void RemoveOneReference()
        {
            if (!Project.Settings.ReferenceChecking) throw new Exception("Reference checking disabled");

            if (IsRuntimeObject && Self is IBaseRuntimeObject obj)
            {
                obj.ReferenceCount.Pointer.With(i =>
                {
                    i.Sub(1);
                    Project.ActiveProject.If(i == 0, () =>
                    {
                        obj.Destruct();
                        Project.ActiveProject.Std.PointerFree(StandardMacros());
                    }).Else(() => obj.ReferenceCount.Pointer.Set(i));

                    return false;
                });
            }
        }

        public IPointer<R> Cast<R>() where R : IPointerable => new RuntimePointer<R>(Pointer.Cast<RuntimePointer<R>>(), ExtraPath);

        public IPointer Cast(Type type)
        {
            return (IPointer?)GetType().GetMethod("Cast", 1, [])?.MakeGenericMethod([type]).Invoke(this, []) ?? throw new Exception("Unable to cast");
        }

        public PointerExists Exists() => new() { Pointer = this };

        public BaseHeapPointer GetHeapPointer() => Pointer.GetHeapPointer();

        public IPointer<T> Local()
        {
            Project.ActiveProject.WithCleanup(Free);
            return this;
        }

        public void Set(ScoreRef val)
        {
            Project.ActiveProject.Std.PointerSet(StandardMacros([new("value", val)]));
        }

        public RuntimePointer<T> ToRTP() => this;

        public RuntimePointer<R> ToRTP<R>() where R : IPointerable => (RuntimePointer<R>)Cast<R>();

        public RuntimePointer<R> ToRTP<R>(RuntimePointer<R> ptr) where R : IPointerable
        {
            ptr.Obj = Obj;
            return ptr;
        }

        public override void Destruct()
        {
            RemoveOneReference();
        }

        internal sealed class Props
        {
            [RuntimeProperty("obj")]
            public NBTString Obj { get; set; }
        }
    }
}
