using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.CubeLib.Builtins.Static;
using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public abstract class BaseHeapPointer : IStandardPointerMacros, IPointer
    {
        public abstract IPointer<R> Cast<R>() where R : Pointerable;
        public abstract void CopyUnsafe(IStandardPointerMacros dest);
        public abstract void Dereference(ScoreRef val);
        public abstract ScoreRef Dereference();
        public abstract void Free();
        public abstract IPointer<R> Get<R>(string path, bool dot = true) where R : Pointerable;
        public abstract ScoreRef GetAsArg();
        public abstract BaseHeapPointer GetHeapPointer();
        public abstract void MoveUnsafe(IStandardPointerMacros dest);
        public abstract void Set(NBTType val);
        public abstract void Set(ScoreRef val);
        public abstract KeyValuePair<string, object>[] StandardMacros(KeyValuePair<string, object>[]? extras = null, string postfix = "");
        public abstract IPointer ToPointer();
        public abstract PointerExists Exists();

        public abstract RuntimePointer<T> ToRTP<T>() where T : Pointerable;
    }

    public class HeapPointer<T>(MCStaticHeap heap, ScoreRef pointer, string extraPath = "") : BaseHeapPointer, IPointer<T> where T : Pointerable
    {
        public readonly MCStaticHeap Heap = heap;
        public readonly ScoreRef Pointer = pointer;

        public readonly string ExtraPath = extraPath;

        public T Self => (T?)typeof(T).GetMethod("Create")?.Invoke(null, [this]) ?? throw new ArgumentException("Not a pointer to a RuntimeObject");

        public override void Set(NBTType val)
        {
            Project.ActiveProject.Std.PointerSet(StandardMacros([new("value", val.ToString())]));
        }

        public override void Set(ScoreRef val)
        {
            Project.ActiveProject.Std.PointerSet(StandardMacros([new("value", val)]));
        }

        public void Copy(IPointer<T> dest) => CopyUnsafe(dest);

        public override void CopyUnsafe(IStandardPointerMacros dest)
        {
            Project.ActiveProject.Std.PointerMove([.. StandardMacros(null, "2"), .. dest.StandardMacros(null, "1")]);
        }

        public void Move(IPointer<T> dest)
        {
            Copy(dest);
            Free();
        }

        public override void MoveUnsafe(IStandardPointerMacros dest)
        {
            CopyUnsafe(dest);
            Free();
        }

        public override IPointer<R> Get<R>(string path, bool dot = true) => new HeapPointer<R>(Heap, Pointer, ExtraPath + (dot ? "." : "") + path);

        public override void Dereference(ScoreRef val) => Project.ActiveProject.Std.PointerDereferenceToScore(StandardMacros(), val);
        public override ScoreRef Dereference()
        {
            var ret = Project.ActiveProject.Local();
            Dereference(ret);
            return ret;
        }

        public override void Free()
        {
            Project.ActiveProject.Std.PointerFree(StandardMacros());
        }

        public HeapPointer<T> Duplicate() => new(Heap, Pointer, ExtraPath);

        public override PointerExists Exists() => new() { Pointer = this };

        public override RuntimePointer<R> ToRTP<R>()
        {
            var ptr = Project.ActiveProject.AllocObj<RuntimePointer<R>>(false);
            Project.ActiveProject.Strcat(ptr.Obj.Pointer, Pointer, ExtraPath);
            return ptr;
        }

        public RuntimePointer<T> ToRTP() => ToRTP<T>();

        public override KeyValuePair<string, object>[] StandardMacros(KeyValuePair<string, object>[]? extras = null, string postfix = "")
        {
            extras ??= [];

            return [new($"storage{postfix}", Heap.Storage),
                new($"path{postfix}", Heap.Path),
                new($"pointer{postfix}", Pointer),
                new($"ext{postfix}", ExtraPath),
                .. extras];
        }

        public override ScoreRef GetAsArg() => (ScoreRef)this;

        public static IRuntimeArgument Create(ScoreRef arg) => new HeapPointer<T>(Project.ActiveProject.Heap, (ScoreRef)ScoreRef.Create(arg));

        public static implicit operator ScoreRef(HeapPointer<T> pointer)
        {
            if (pointer.ExtraPath != "") throw new Exception("Pointers with subpaths cannot be converted to a score");
            return pointer.Pointer;
        }

        public static void CheckValidType()
        {
            if (NBTType.IsNBTType<T>()) return;
            if (typeof(T).IsSubclassOf(typeof(IBaseRuntimeObject))) return;
            if (typeof(T).IsSubclassOf(typeof(IRuntimeArgument))) return;

            throw new ArgumentException($"Type {typeof(T).Name} is not a valid HeapPointerProperty type");
        }

        public override IPointer ToPointer() => this;

        public override IPointer<R> Cast<R>() => new HeapPointer<R>(Heap, Pointer, ExtraPath);

        public override BaseHeapPointer GetHeapPointer() => this;

        public IPointer<T> Local()
        {
            Project.ActiveProject.WithCleanup(Free);
            return this;
        }
    }
}
