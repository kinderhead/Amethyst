using System;

namespace Datapack.Net.CubeLib
{
    public class RuntimeProperty<T> : IRuntimeProperty<T>
    {
        public IPointer<T> Pointer { get; protected set; }
        public T PropValue { get; }

        public RuntimeProperty(IPointer<T> pointer)
        {
            Pointer = pointer;
        }

        public RuntimeProperty(T val)
        {
            PropValue = val;
        }

        public void Copy(IPointer<T> dest) => (Pointer ?? throw new Exception("RuntimeProperty is not fully qualified")).Copy(dest);

        public static implicit operator RuntimeProperty<T>(T val) => new(val);

        public IPointer ToPointer() => Pointer;
    }
}
