using System.Text;

namespace Amethyst.IR
{
    public interface ISerializableType
    {
        void Serialize(StringBuilder builder);
    }
}