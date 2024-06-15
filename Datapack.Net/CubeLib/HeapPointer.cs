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
    public abstract class BaseHeapPointer : IStandardPointerMacros
    {
        public abstract KeyValuePair<string, object>[] StandardMacros(KeyValuePair<string, object>[]? extras = null, string postfix = "");
        public abstract RuntimePointer<T> ToRTP<T>();
    }

    public class HeapPointer<T>(MCStaticHeap heap, ScoreRef pointer, string extraPath = "") : BaseHeapPointer, IPointer<T>
    {
        public readonly MCStaticHeap Heap = heap;
        public readonly ScoreRef Pointer = pointer;

        public readonly string ExtraPath = extraPath;

        public T Self => (T?)typeof(T).GetMethod("Create")?.Invoke(null, [this]) ?? throw new ArgumentException("Not a pointer to a RuntimeObject");

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

        public IPointer<R> Get<R>(string path, bool dot = true) => new HeapPointer<R>(Heap, Pointer, ExtraPath + (dot ? "." : "") + path);

        public void Dereference(ScoreRef val) => Project.ActiveProject.Std.PointerDereferenceToScore(StandardMacros(), val);
        public ScoreRef Dereference()
        {
            var ret = Project.ActiveProject.Local();
            Dereference(ret);
            return ret;
        }

        public void Free()
        {
            Project.ActiveProject.Std.PointerFree(StandardMacros());
        }

        public HeapPointer<T> Duplicate() => new(Heap, Pointer, ExtraPath);

        public PointerExists<T> Exists() => new() { Pointer = this };

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

        public ScoreRef GetAsArg() => (ScoreRef)this;

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

        public IPointer ToPointer() => this;

        public IPointer<R> Cast<R>() => new HeapPointer<R>(Heap, Pointer, ExtraPath);
    }
}
