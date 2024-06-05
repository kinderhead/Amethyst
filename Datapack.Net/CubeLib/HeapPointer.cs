using Datapack.Net.CubeLib.Builtins.Static;
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
    public abstract class BaseHeapPointer
    {
        public abstract KeyValuePair<string, object>[] StandardMacros(KeyValuePair<string, object>[]? extras = null, string postfix = "");
    }

    public class HeapPointer<T>(MCStaticHeap heap, ScoreRef pointer, string extraPath = "") : BaseHeapPointer, IRuntimeArgument
    {
        public readonly MCStaticHeap Heap = heap;
        public readonly ScoreRef Pointer = pointer;

        public readonly string ExtraPath = extraPath;

        public void Set(NBTType val)
        {
            var obj = val is NBTString ? val.ToString() : val;

            Project.ActiveProject.Std.PointerSet(StandardMacros([new("value", obj)]));
        }

        public void Copy(HeapPointer<T> dest) => CopyUnsafe(dest);

        public void CopyUnsafe(BaseHeapPointer dest)
        {
            Project.ActiveProject.Std.PointerMove([.. StandardMacros(null, "2"), .. dest.StandardMacros(null, "1")]);
        }

        public void Move(HeapPointer<T> dest)
        {
            Copy(dest);
            Free();
        }

        public void MoveUnsafe(BaseHeapPointer dest)
        {
            CopyUnsafe(dest);
            Free();
        }

        public HeapPointer<R> Get<R>(string path) => new(Heap, Pointer, ExtraPath + "." + path);

        public void Dereference(ScoreRef val) => Project.ActiveProject.Std.PointerDereference(StandardMacros(), val);
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

        public PointerExists<T> Exists() => new() { Pointer = this };

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
    }
}
