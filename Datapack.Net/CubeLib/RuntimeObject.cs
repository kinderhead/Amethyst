using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;
using System.Reflection;

namespace Datapack.Net.CubeLib
{
	public interface IBaseRuntimeObject : IRuntimeArgument, IPointerable
	{
		bool Freed { get; }

		RuntimeProperty<NBTInt> ReferenceCount { get; set; }

		void IfNull(Action func);
		bool HasMethod(string name);
		Delegate GetMethod(string name);

		void FreeObj();

		void Destruct();

		IPointer GetPointer();

		static abstract T Create<T>(IPointer<T> pointer) where T : IPointerable;

		static IRuntimeArgument Create(ScoreRef arg, Type self) => (IRuntimeArgument?)Activator.CreateInstance(self, [typeof(HeapPointer<>).MakeGenericType(self).GetMethod("Create")?.Invoke(null, [arg])]) ?? throw new ArgumentException("Error dynamically creating a RuntimeObject");
		static IRuntimeArgument CreateWithRTP(ScoreRef loc, Type self) => (IRuntimeArgument?)Activator.CreateInstance(self, [Create(loc, typeof(RuntimePointer<>).MakeGenericType(self))]) ?? throw new ArgumentException("Error dynamically creating a RuntimeObject");
	}

	public abstract class RuntimeObject<TProject, TSelf> : IBaseRuntimeObject, IRuntimeProperty<TSelf> where TProject : Project where TSelf : RuntimeObject<TProject, TSelf>
	{
		public IPointer<TSelf> Pointer { get; }
		public IPointer GetPointer() => Pointer;

		public TSelf PropValue => (TSelf)this;

		public bool Freed { get; protected set; }

		public RuntimeProperty<NBTInt> ReferenceCount
		{
			get
			{
				if (!Project.Settings.ReferenceChecking)
				{
					throw new Exception("Reference checking disabled");
				}

				return new(GetProp<NBTInt>("__ref"));
			}
			set
			{
				if (!Project.Settings.ReferenceChecking)
				{
					throw new Exception("Reference checking disabled");
				}

				SetProp("__ref", value);
			}
		}

		public RuntimeObject(IPointer<TSelf> loc)
		{
			Pointer = loc;
		}

		public RuntimeObject() { }

		public virtual (string, Type)[] AllProperties { get; }

		protected IPointer<T> GetProp<T>(string path, bool dot = true) where T : IPointerable => Pointer.Get<T>(path, dot);
		protected T GetObj<T>(string path, bool dot = true) where T : IBaseRuntimeObject => T.Create(RuntimePointer<T>.Create(Pointer.Get<RuntimePointer<T>>(path, dot)));

		protected void SetProp(string path, NBTValue val) => Pointer.Get<NBTValue>(path).Set(val);
		protected void SetProp<T>(string path, IPointer<T> pointer) where T : IPointerable
		{
			if (typeof(T).IsAssignableTo(typeof(IBaseRuntimeObject)))
			{
				var place = GetProp<T>(path).Get<NBTString>("obj");
				if (Project.Settings.ReferenceChecking)
				{
					((IBaseRuntimeObject)pointer.Self).ReferenceCount.Pointer.With(i => i.Add(1));
				}

				Project.ActiveProject.Std.StorePointer([.. place.StandardMacros([], "1"), .. pointer.StandardMacros([], "2")]);
			}
			else
			{
				pointer.Copy(GetProp<T>(path));
			}
		}

		protected void SetProp<T>(string path, IRuntimeProperty<T> prop) where T : IPointerable
		{
			if (prop.Pointer is not null)
			{
				SetProp(path, prop.Pointer);
			}
			else if (NBTValue.IsNBTType<T>())
			{
				SetProp(path, NBTValue.ToNBT(prop.PropValue ?? throw new ArgumentException("RuntimeProperty was not created properly")) ?? throw new Exception("How did we get here?"));
			}
			else
			{
				throw new ArgumentException("RuntimeProperty was not created properly");
			}
		}

		public virtual void FreeObj()
		{
			Pointer.Free();
			Freed = true;
		}

		public void CopyObj(IPointer<TSelf> dest) => Pointer.Copy(dest);
		public void MoveObj(IPointer<TSelf> dest) => Pointer.Move(dest);
		public void MoveObj(TSelf dest) => Pointer.Move(dest.Pointer);

		public void IfNull(Action func)
		{
			if (Pointer is HeapPointer<TSelf> hp)
			{
				_ = State.If(hp.Exists(), func);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public ScoreRef GetAsArg() => Pointer.GetAsArg();

		public bool HasMethod(string name)
		{
			foreach (var i in GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
			{
				if (i.GetCustomAttribute<DeclareMCAttribute>()?.Path == name)
				{
					return true;
				}
			}

			return false;
		}

		public Delegate GetMethod(string name)
		{
			foreach (var i in GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
			{
				if (i.GetCustomAttribute<DeclareMCAttribute>()?.Path == name)
				{
					return DelegateUtils.Create(i, null);
				}
			}

			throw new Exception($"Cannot get method {name} from object");
		}

		public virtual void Destruct()
		{
			if (!Project.Settings.ReferenceChecking)
			{
				throw new Exception("Reference checking disabled");
			}

			if (HasMethod("deinit"))
			{
				Project.ActiveProject.AddCommand(Project.ActiveProject.Lambda(() => Project.ActiveProject.CallArg(GetMethod("deinit"), [this])));
			}
		}

		public static TProject State
		{
			get
			{
				field ??= Project.Create<TProject>(Project.ActiveProject.Datapack);
				return field;
			}
		} = null;

		public static T Create<T>(IPointer<T> pointer) where T : IPointerable => (T?)Activator.CreateInstance(typeof(TSelf), [pointer]) ?? throw new ArgumentException("Failed to create runtime object");

		public static IRuntimeArgument Create(ScoreRef arg) => Create((IPointer<TSelf>)HeapPointer<TSelf>.Create(arg));

		public static implicit operator RuntimeObject<TProject, TSelf>(HeapPointer<TSelf> pointer) => Create<TSelf>(pointer);

		public virtual IPointer ToPointer() => Pointer;
	}
}
