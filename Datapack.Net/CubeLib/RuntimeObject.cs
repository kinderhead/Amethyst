using System;
using System.Reflection;
using Datapack.Net.CubeLib.Builtins;
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

        public IPointer GetPointer();

        public static abstract IBaseRuntimeObject Create<T>(IPointer<T> pointer);

        public static IRuntimeArgument Create(ScoreRef arg, Type self) => (IRuntimeArgument?)Activator.CreateInstance(self, [typeof(HeapPointer<>).MakeGenericType(self).GetMethod("Create")?.Invoke(null, [arg])]) ?? throw new ArgumentException("Error dynamically creating a RuntimeObject");
        public static IRuntimeArgument CreateWithRTP(ScoreRef loc, Type self) => (IRuntimeArgument?)Activator.CreateInstance(self, [Create(loc, typeof(RuntimePointer<>).MakeGenericType(self))]) ?? throw new ArgumentException("Error dynamically creating a RuntimeObject");
    }

    public abstract class RuntimeObject<TProject, TSelf> : IBaseRuntimeObject, IRuntimeProperty<TSelf> where TProject : Project where TSelf : RuntimeObject<TProject, TSelf>
    {
        public IPointer<TSelf> Pointer { get; }
        public IPointer GetPointer() => Pointer;

        public TSelf Value { get => (TSelf)this; }

        public RuntimeObject(IPointer<TSelf> loc)
        {
            Pointer = loc;
        }

        public RuntimeObject() {}

        protected IPointer<T> GetProp<T>(string path) => Pointer.Get<T>(path);
        protected T GetObj<T>(string path) where T : IBaseRuntimeObject => (T)T.Create((RuntimePointer<T>)RuntimePointer<T>.Create(Pointer.Get<RuntimePointer<T>>(path)));

        protected void SetProp(string path, NBTType val) => Pointer.Get<NBTType>(path).Set(val);
        protected void SetProp<T>(string path, IPointer<T> pointer)
        {
            var place = GetProp<T>(path).Get<string>("obj");
            Project.ActiveProject.Std.StorePointer([.. place.StandardMacros([], "1"), .. pointer.StandardMacros([], "2")]);
        }
        protected void SetProp<T>(string path, IRuntimeProperty<T> prop)
        {
            if (prop.Pointer is not null) SetProp(path, prop.Pointer);
            else if (NBTType.IsNBTType<T>()) SetProp(path, NBTType.ToNBT(prop.Value ?? throw new ArgumentException("RuntimeProperty was not created properly")) ?? throw new Exception("How did we get here?"));
            else throw new ArgumentException("RuntimeProperty was not created properly");
        }

        public void FreeObj() => Pointer.Free();
        public void CopyObj(IPointer<TSelf> dest) => Pointer.Copy(dest);
        public void MoveObj(IPointer<TSelf> dest) => Pointer.Move(dest);
        public void MoveObj(TSelf dest) => Pointer.Move(dest.Pointer);

        public void IfNull(Action func)
        {
            if (Pointer is HeapPointer<TSelf> hp) State.If(hp.Exists(), func);
            else throw new NotImplementedException();
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

        public static IBaseRuntimeObject Create<T>(IPointer<T> pointer)
        {
            return (IBaseRuntimeObject?)Activator.CreateInstance(typeof(TSelf), [pointer]) ?? throw new ArgumentException("Failed to create runtime object");
        }

        public static IRuntimeArgument Create(ScoreRef arg) => Create((IPointer<TSelf>)HeapPointer<TSelf>.Create(arg));

        public static implicit operator RuntimeObject<TProject, TSelf>(HeapPointer<TSelf> pointer) => (RuntimeObject<TProject, TSelf>)Create<TSelf>(pointer);

        public virtual IPointer ToPointer() => Pointer;
    }
}
