using System;

namespace Datapack.Net.CubeLib
{
    public class RuntimeProperty<T> : IRuntimeProperty<T>
    {
        public HeapPointer<T> Pointer { get; protected set; }
        public T Value { get; }

        internal RuntimeProperty(HeapPointer<T> pointer)
        {
            Pointer = pointer;
        }

        internal RuntimeProperty(T val)
        {
            Value = val;
        }

        public void Copy(HeapPointer<T> dest) => (Pointer ?? throw new Exception("RuntimeProperty is not fully qualified")).Copy(dest);

        public static implicit operator RuntimeProperty<T>(T val) => new(val);
        public static implicit operator HeapPointer<T>(RuntimeProperty<T> prop) => prop.Pointer ?? throw new Exception("RuntimeProperty is not fully qualified");
        public static implicit operator RuntimeProperty<T>(HeapPointer<T> pointer) => new(pointer);
    }
}
