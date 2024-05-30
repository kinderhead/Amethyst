using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public class HeapPointer(MCHeap heap, ScoreRef pointer) : IRuntimeArgument
    {
        public readonly MCHeap Heap = heap;
        public readonly ScoreRef Pointer = pointer;

        public void Set(int val)
        {
            Project.ActiveProject.Call(Project.ActiveProject.Std.PointerSet, StandardMacros([new("value", val)]));
        }

        public void Dereference(ScoreRef val) => Project.ActiveProject.CallRet(Project.ActiveProject.Std.PointerDereference, val, StandardMacros());
        public ScoreRef Dereference()
        {
            var ret = Project.ActiveProject.Local();
            Dereference(ret);
            return ret;
        }

        public void Free()
        {
            Project.ActiveProject.Call(Project.ActiveProject.Std.PointerFree, StandardMacros());
        }

        public PointerExists Exists() => new() { Pointer = this };

        public KeyValuePair<string, object>[] StandardMacros(KeyValuePair<string, object>[]? extras = null)
        {
            extras ??= [];
            
            return [new("storage", Heap.Storage),
                new("path", Heap.Path),
                new("pointer", Pointer),
                .. extras];
        }

        public ScoreRef GetAsArg() => Pointer;

        public static implicit operator ScoreRef(HeapPointer pointer) => pointer.Pointer;
    }
}
