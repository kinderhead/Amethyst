using System;
using System.Reflection;
using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;
using Datapack.Net.Function;

namespace Datapack.Net.CubeLib
{
    public interface IBaseRuntimeObject : IRuntimeArgument
    {
        public void IfNull(Action func);
        public bool HasMethod(string name);
        public Delegate GetMethod(string name);

        public static abstract IBaseRuntimeObject Create(BaseHeapPointer pointer);

        public static IRuntimeArgument Create(ScoreRef arg, Type self) => (IRuntimeArgument?)Activator.CreateInstance(self, [typeof(HeapPointer<>).MakeGenericType(self).GetMethod("Create")?.Invoke(null, [arg])]) ?? throw new ArgumentException("Error dynamically creating a RuntimeObject");
    }

    public abstract class RuntimeObject<TProject, TSelf> : IBaseRuntimeObject, IRuntimeProperty<TSelf> where TProject : Project where TSelf : RuntimeObject<TProject, TSelf>
    {
        public HeapPointer<TSelf> Pointer { get; }

        public TSelf Value { get => (TSelf)this; }

        public RuntimeObject(HeapPointer<TSelf> loc)
        {
            Pointer = loc;
        }

        public RuntimeObject() {}

        protected HeapPointer<T> Get<T>(string path) => Pointer.Get<T>(path);
        protected T GetObj<T>(string path) where T : IBaseRuntimeObject => (T)T.Create(Pointer.Get<T>(path));

        protected void Set(string path, NBTType val) => Pointer.Get<NBTType>(path).Set(val);
        protected void Set<T>(string path, HeapPointer<T> pointer) => pointer.Copy(Get<T>(path));
        protected void Set<T>(string path, IRuntimeProperty<T> prop)
        {
            if (prop.Pointer is not null) Set(path, prop.Pointer);
            else if (NBTType.IsNBTType<T>()) Set(path, NBTType.ToNBT(prop.Value ?? throw new ArgumentException("RuntimeProperty was not created properly")) ?? throw new Exception("How did we get here?"));
            else throw new ArgumentException("RuntimeProperty was not created properly");
        }

        public void Free() => Pointer.Free();
        public void Copy(HeapPointer<TSelf> dest) => Pointer.Copy(dest);
        public void Move(HeapPointer<TSelf> dest) => Pointer.Move(dest);
        public void Move(TSelf dest) => Pointer.Move(dest.Pointer);

        public void IfNull(Action func)
        {
            State.If(Pointer.Exists(), func);
        }

        public ScoreRef GetAsArg() => Pointer.GetAsArg();

        public bool HasMethod(string name)
        {
            foreach (var i in GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (i.GetCustomAttribute<DeclareMCAttribute>()?.Path == name) return true;
            }

            return false;
        }

        public Delegate GetMethod(string name)
        {
            foreach (var i in GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (i.GetCustomAttribute<DeclareMCAttribute>()?.Path == name) return DelegateUtils.Create(i, null);
            }

            throw new Exception($"Cannot get method {name} from object");
        }

        private static TProject? _state = null;
        public static TProject State
        {
            get
            {
                _state ??= Project.Create<TProject>(Project.ActiveProject.Datapack);
                return _state;
            }
        }

        public static IBaseRuntimeObject Create(BaseHeapPointer pointer)
        {
            return (IBaseRuntimeObject?)Activator.CreateInstance(typeof(TSelf), [pointer]) ?? throw new ArgumentException("Failed to create runtime object");
        }

        public static IRuntimeArgument Create(ScoreRef arg) => Create((BaseHeapPointer)HeapPointer<TSelf>.Create(arg));

        public static implicit operator RuntimeObject<TProject, TSelf>(HeapPointer<TSelf> pointer) => (RuntimeObject<TProject, TSelf>)Create((BaseHeapPointer)pointer);
    }
}
