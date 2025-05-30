using System;
using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;

namespace Datapack.Net.CubeLib
{
    public class MacroPointer<T>(string storage, string path, string pointer, string ext) : IPointer<T> where T : IPointerable
    {
        public readonly string Storage = storage;
        public readonly string Path = path;
        public readonly string Pointer = pointer;
        public readonly string Ext = ext;

        public T Self => (T?)typeof(T).GetMethod("Create")?.Invoke(null, [this]) ?? throw new ArgumentException("Not a pointer to a RuntimeObject");

        public IPointer<R> Cast<R>() where R : IPointerable => new MacroPointer<R>(Storage, Path, Pointer, Ext);

        public IPointer Cast(Type type) => (IPointer?)GetType().GetMethod("Cast", 1, [])?.MakeGenericMethod([type]).Invoke(this, []) ?? throw new Exception("Unable to cast");

        public void Copy(IPointer<T> dest) => CopyUnsafe(dest);

        public void CopyUnsafe(IStandardPointerMacros dest) => Project.ActiveProject.Std.PointerMove([.. StandardMacros(null, "2"), .. dest.StandardMacros(null, "1")], true);

        public void Dereference(ScoreRef val) => Project.ActiveProject.Std.PointerDereferenceToScore(StandardMacros(), val, true);

        public ScoreRef Dereference()
        {
            var ret = Project.ActiveProject.Local();
            Dereference(ret);
            return ret;
        }

        public PointerExists Exists() => new() { Pointer = this };

        public void Free()
        {
            throw new NotImplementedException();
        }

        public IPointer<R> Get<R>(string path, bool dot = true) where R : IPointerable
        {
            throw new NotImplementedException();
        }

        public ScoreRef GetAsArg()
        {
            throw new NotImplementedException();
        }

        public BaseHeapPointer GetHeapPointer()
        {
            throw new NotImplementedException();
        }

        public IPointer<T> Local()
        {
            throw new NotImplementedException();
        }

        public void Move(IPointer<T> dest)
        {
            throw new NotImplementedException();
        }

        public void MoveUnsafe(IStandardPointerMacros dest)
        {
            throw new NotImplementedException();
        }

        public void Set(NBTValue val)
        {
            throw new NotImplementedException();
        }

        public void Set(ScoreRef val)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<string, object>[] StandardMacros(KeyValuePair<string, object>[]? extras = null, string postfix = "")
        {
            throw new NotImplementedException();
        }

        public IPointer ToPointer()
        {
            throw new NotImplementedException();
        }

        public RuntimePointer<T> ToRTP()
        {
            throw new NotImplementedException();
        }

        public RuntimePointer<R> ToRTP<R>() where R : IPointerable
        {
            throw new NotImplementedException();
        }

        public RuntimePointer<R> ToRTP<R>(RuntimePointer<R> ptr) where R : IPointerable
        {
            throw new NotImplementedException();
        }
    }
}
