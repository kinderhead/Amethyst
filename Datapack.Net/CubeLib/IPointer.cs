using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;

namespace Datapack.Net.CubeLib
{
	public interface IPointer : IStandardPointerMacros, IRuntimeArgument, IToPointer
	{
		void CopyUnsafe(IStandardPointerMacros dest);
		void Dereference(ScoreRef val);
		ScoreRef Dereference();
		void Free();
		IPointer<R> Get<R>(string path, bool dot = true) where R : IPointerable;
		void MoveUnsafe(IStandardPointerMacros dest);
		void Set(NBTValue val);
		void Set(ScoreRef val);
		IPointer<R> Cast<R>() where R : IPointerable;
		IPointer Cast(Type type);
		RuntimePointer<R> ToRTP<R>() where R : IPointerable;
		RuntimePointer<R> ToRTP<R>(RuntimePointer<R> ptr) where R : IPointerable;

		BaseHeapPointer GetHeapPointer();
		PointerExists Exists();
	}

	public interface IPointer<T> : IPointer where T : IPointerable
	{
		T Self { get; }
		void Copy(IPointer<T> dest);
		void Move(IPointer<T> dest);
		IPointer<T> Local();
		RuntimePointer<T> ToRTP();
	}

	public interface IPointerable
	{

	}

	public static class PointerExtension
	{
		/// <summary>
		/// Temporarily use the pointer as a score. If <c>func</c> returns true, then the pointer will be updated with the score.
		/// </summary>
		/// <param name="ptr">Pointer</param>
		/// <param name="func">Callback function</param>
		/// <param name="tmp">Temp count</param>
		public static void With(this IPointer<NBTInt> ptr, Func<ScoreRef, bool> func, int tmp = 0)
		{
			var math = Project.ActiveProject.Temp(tmp, "ptr_math");
			ptr.Dereference(math);
			if (func(math))
			{
				ptr.Set(math);
			}
		}

		public static void With(this IPointer<NBTInt> ptr, Action<ScoreRef> func, int tmp = 0) => ptr.With(i =>
																										   {
																											   func(i);
																											   return true;
																										   }, tmp);
	}
}
