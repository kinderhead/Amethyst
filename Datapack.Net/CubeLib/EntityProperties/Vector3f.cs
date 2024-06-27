using System;
using Datapack.Net.Data;

namespace Datapack.Net.CubeLib.EntityProperties
{
    public class Vector3f() : EntityStructure<NBTList<NBTFloat>>
    {
        public FloatEntityProperty X { get => GetProp<FloatEntityProperty>("[0]"); set => SetProp("[0]", value); }
        public FloatEntityProperty Y { get => GetProp<FloatEntityProperty>("[1]"); set => SetProp("[1]", value); }
        public FloatEntityProperty Z { get => GetProp<FloatEntityProperty>("[2]"); set => SetProp("[2]", value); }

        public Vector3f(float x, float y, float z) : this()
        {
            Value = [x, y, z];
        }
    }
}
