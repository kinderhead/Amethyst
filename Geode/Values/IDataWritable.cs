using System;

namespace Geode.Values
{
    public interface IDataWritable
    {
        void StoreTo(DataTargetValue val, RenderContext ctx);
    }
}
