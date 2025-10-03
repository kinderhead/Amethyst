using Datapack.Net.CubeLib.Utils;

namespace Datapack.Net.CubeLib
{
	public interface IRuntimeProperty<T> : IToPointer where T : IPointerable
	{
		IPointer<T> Pointer { get; }
		T PropValue { get; }
	}
}
