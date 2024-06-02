using System;

namespace Datapack.Net.CubeLib
{
    public interface IRuntimeProperty<T>
    {
        public HeapPointer<T> Pointer { get; }
        public T Value { get; }
    }
}
