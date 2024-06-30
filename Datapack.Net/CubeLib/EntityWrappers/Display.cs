using System;
using Datapack.Net.CubeLib.EntityProperties;
using Datapack.Net.Data;

namespace Datapack.Net.CubeLib.EntityWrappers
{
    public abstract class Display(ScoreRef id) : EntityWrapper(id)
    {
        public StringEntityProperty RawBillboard { get => GetAs<StringEntityProperty>("billboard"); set => value.Set(this, "billboard"); }
        public Vector3f Translation { get => GetAs<Vector3f>("transformation.translation"); set => value.Set(this, "transformation.translation"); }
        public Vector3f Scale { get => GetAs<Vector3f>("transformation.scale"); set => value.Set(this, "transformation.scale"); }
        public FloatEntityProperty InterpolationDuration { get => GetAs<FloatEntityProperty>("interpolation_duration"); set => value.Set(this, "interpolation_duration"); }
        public FloatEntityProperty InterpolationStart { get => GetAs<FloatEntityProperty>("interpolation_start"); set => value.Set(this, "interpolation_start"); }

        public void SetBillboard(Billboard type) => RawBillboard = Enum.GetName(typeof(Billboard), type)?.ToLower() ?? throw new ArgumentException("Invalid enum");

        public void StartAnimation() => InterpolationStart = 0;
    }

    public enum Billboard
    {
        Fixed,
        Horizontal,
        Vertical,
        Center
    }
}
