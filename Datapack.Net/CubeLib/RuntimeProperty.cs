using System;

namespace Datapack.Net.CubeLib
{
    public class RuntimeProperty<T>
    {
        public readonly HeapPointer<T>? Pointer;
        public readonly T Value;

        internal RuntimeProperty(HeapPointer<T> pointer)
        {
            Pointer = pointer;
        }

        internal RuntimeProperty(T val)
        {
            Value = val;
        }

        public static implicit operator RuntimeProperty<T>(T val) => new(val);
        public static implicit operator HeapPointer<T>(RuntimeProperty<T> prop) => prop.Pointer ?? throw new Exception("RuntimeProperty is not fully qualified");
        public static implicit operator RuntimeProperty<T>(HeapPointer<T> pointer) => new(pointer);
    }
}
