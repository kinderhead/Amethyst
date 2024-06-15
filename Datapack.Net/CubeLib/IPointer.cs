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
        public IPointer<R> Get<R>(string path, bool dot = true);
        public void MoveUnsafe(IStandardPointerMacros dest);
        public void Set(NBTType val);
        public IPointer<R> Cast<R>();
    }

    public interface IPointer<T> : IPointer
    {
        public T Self { get; }
        public void Copy(IPointer<T> dest);
        public void Move(IPointer<T> dest);
    }
}
