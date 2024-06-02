using Datapack.Net.CubeLib.Builtins;
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
        
    }

    public class HeapPointer<T>(MCHeap heap, ScoreRef pointer, string extraPath = "") : BaseHeapPointer, IRuntimeArgument
    {
        public readonly MCHeap Heap = heap;
        public readonly ScoreRef Pointer = pointer;

        public readonly string ExtraPath = extraPath;

        public void Set(int val)
        {
            Project.ActiveProject.Call(Project.ActiveProject.Std._PointerSet, StandardMacros([new("value", val)]));
        }

        public void Set(NBTType val)
        {
            Project.ActiveProject.Call(Project.ActiveProject.Std._PointerSet, StandardMacros([new("value", val)]));
        }

        public void Move<R>(HeapPointer<R> dest)
        {
            Project.ActiveProject.Call(Project.ActiveProject.Std._PointerMove, [.. StandardMacros(null, "1"), .. dest.StandardMacros(null, "2")]);
        }

        public HeapPointer<R> Get<R>(string path) => new(Heap, Pointer, ExtraPath + "." + path);

        public void Dereference(ScoreRef val) => Project.ActiveProject.CallRet(Project.ActiveProject.Std._PointerDereference, val, StandardMacros());
        public ScoreRef Dereference()
        {
            var ret = Project.ActiveProject.Local();
            Dereference(ret);
            return ret;
        }

        public void Free()
        {
            Project.ActiveProject.Call(Project.ActiveProject.Std._PointerFree, StandardMacros());
        }

        public PointerExists<T> Exists() => new() { Pointer = this };

        public KeyValuePair<string, object>[] StandardMacros(KeyValuePair<string, object>[]? extras = null, string postfix = "")
        {
            extras ??= [];

            return [new($"storage{postfix}", Heap.Storage),
                new($"path{postfix}", Heap.Path),
                new($"pointer{postfix}", Pointer),
                new($"ext{postfix}", ExtraPath),
                .. extras];
        }

        public ScoreRef GetAsArg() => Pointer;

        public static IRuntimeArgument Create(ScoreRef arg) => new HeapPointer<T>(Project.ActiveProject.Heap, (ScoreRef)ScoreRef.Create(arg));

        public static implicit operator ScoreRef(HeapPointer<T> pointer) => pointer.Pointer;

        public static void CheckValidType()
        {
            if (NBTType.IsNBTType<T>()) return;
            if (typeof(T).IsSubclassOf(typeof(IBaseRuntimeObject))) return;
            if (typeof(T).IsSubclassOf(typeof(IRuntimeArgument))) return;

            throw new ArgumentException($"Type {typeof(T).Name} is not a valid HeapPointerProperty type");
        }
    }
}
