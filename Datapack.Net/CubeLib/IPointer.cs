using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public interface IPointer : IStandardPointerMacros, IRuntimeArgument, IToPointer
    {
        public void CopyUnsafe(IStandardPointerMacros dest);
        public void Dereference(ScoreRef val);
        public ScoreRef Dereference();
        public void Free();
        public IPointer<R> Get<R>(string path, bool dot = true) where R : IPointerable;
        public void MoveUnsafe(IStandardPointerMacros dest);
        public void Set(NBTType val);
        public void Set(ScoreRef val);
        public IPointer<R> Cast<R>() where R : IPointerable;
        public IPointer Cast(Type type);
        public RuntimePointer<R> ToRTP<R>() where R : IPointerable;
        public RuntimePointer<R> ToRTP<R>(RuntimePointer<R> ptr) where R : IPointerable;

        public BaseHeapPointer GetHeapPointer();
        public PointerExists Exists();
    }

    public interface IPointer<T> : IPointer where T : IPointerable
    {
        public T Self { get; }
        public void Copy(IPointer<T> dest);
        public void Move(IPointer<T> dest);
        public IPointer<T> Local();
        public RuntimePointer<T> ToRTP();
    }

    public interface IPointerable
    {

    }

    public static class PointerExtension
    {
        /// <summary>
        /// Temporarily use the pointer as a score. If <c>func</c> returns true, then the pointer will be updated with the score.
        /// </summary>
        /// <param name="ptr">Pointer</param>
        /// <param name="func">Callback function</param>
        /// <param name="tmp">Temp count</param>
        public static void With(this IPointer<NBTInt> ptr, Func<ScoreRef, bool> func, int tmp = 0)
        {
            var math = Project.ActiveProject.Temp(tmp, "ptr_math");
            ptr.Dereference(math);
            if (func(math)) ptr.Set(math);
        }

        public static void With(this IPointer<NBTInt> ptr, Action<ScoreRef> func, int tmp = 0)
        {
            ptr.With(i =>
            {
                func(i);
                return true;
            }, tmp);
        }
    }
}
