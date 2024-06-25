using Datapack.Net.Data;
using System;

namespace Datapack.Net.CubeLib
{
    public class RuntimeProperty<T> : IRuntimeProperty<T> where T : IPointerable
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

    public class IntRuntimeProperty : RuntimeProperty<NBTInt>
    {
        public IntRuntimeProperty(IPointer<NBTInt> pointer) : base(pointer)
        {
        }

        public IntRuntimeProperty(NBTInt val) : base(val)
        {
        }

        public static implicit operator IntRuntimeProperty(int val) => new(val);
    }

    public class LongRuntimeProperty : RuntimeProperty<NBTLong>
    {
        public LongRuntimeProperty(IPointer<NBTLong> pointer) : base(pointer)
        {
        }

        public LongRuntimeProperty(NBTLong val) : base(val)
        {
        }

        public static implicit operator LongRuntimeProperty(long val) => new(val);
    }

    public class FloatRuntimeProperty : RuntimeProperty<NBTFloat>
    {
        public FloatRuntimeProperty(IPointer<NBTFloat> pointer) : base(pointer)
        {
        }

        public FloatRuntimeProperty(NBTFloat val) : base(val)
        {
        }

        public static implicit operator FloatRuntimeProperty(float val) => new(val);
    }

    public class DoubleRuntimeProperty : RuntimeProperty<NBTDouble>
    {
        public DoubleRuntimeProperty(IPointer<NBTDouble> pointer) : base(pointer)
        {
        }

        public DoubleRuntimeProperty(NBTDouble val) : base(val)
        {
        }

        public static implicit operator DoubleRuntimeProperty(double val) => new(val);
    }

    public class ShortRuntimeProperty : RuntimeProperty<NBTShort>
    {
        public ShortRuntimeProperty(IPointer<NBTShort> pointer) : base(pointer)
        {
        }

        public ShortRuntimeProperty(NBTShort val) : base(val)
        {
        }

        public static implicit operator ShortRuntimeProperty(short val) => new(val);
    }

    public class BoolRuntimeProperty : RuntimeProperty<NBTBool>
    {
        public BoolRuntimeProperty(IPointer<NBTBool> pointer) : base(pointer)
        {
        }

        public BoolRuntimeProperty(NBTBool val) : base(val)
        {
        }

        public static implicit operator BoolRuntimeProperty(bool val) => new(val);
    }

    public class ByteRuntimeProperty : RuntimeProperty<NBTByte>
    {
        public ByteRuntimeProperty(IPointer<NBTByte> pointer) : base(pointer)
        {
        }

        public ByteRuntimeProperty(NBTByte val) : base(val)
        {
        }

        public static implicit operator ByteRuntimeProperty(byte val) => new(val);
    }

    public class StringRuntimeProperty : RuntimeProperty<NBTString>
    {
        public StringRuntimeProperty(IPointer<NBTString> pointer) : base(pointer)
        {
        }

        public StringRuntimeProperty(NBTString val) : base(val)
        {
        }

        public static implicit operator StringRuntimeProperty(string val) => new(val);
    }
}
