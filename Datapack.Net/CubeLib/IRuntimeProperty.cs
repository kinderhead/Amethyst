using Datapack.Net.CubeLib.Utils;
using System;

namespace Datapack.Net.CubeLib
{
    public interface IRuntimeProperty<T> : IToPointer
    {
        public IPointer<T> Pointer { get; }
        public T PropValue { get; }
    }
}
