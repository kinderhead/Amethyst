using System;
using System.Reflection;
using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;
using Datapack.Net.Function;

namespace Datapack.Net.CubeLib
{
    public interface IBaseRuntimeObject : IRuntimeArgument, Pointerable
    {
        public bool Freed { get; }

        public RuntimeProperty<NBTInt> ReferenceCount { get; set; }

        public void IfNull(Action func);
        public bool HasMethod(string name);
        public Delegate GetMethod(string name);

        public void FreeObj();

        public IPointer GetPointer();

        public static abstract IBaseRuntimeObject Create<T>(IPointer<T> pointer) where T : Pointerable;

        public static IRuntimeArgument Create(ScoreRef arg, Type self) => (IRuntimeArgument?)Activator.CreateInstance(self, [typeof(HeapPointer<>).MakeGenericType(self).GetMethod("Create")?.Invoke(null, [arg])]) ?? throw new ArgumentException("Error dynamically creating a RuntimeObject");
        public static IRuntimeArgument CreateWithRTP(ScoreRef loc, Type self) => (IRuntimeArgument?)Activator.CreateInstance(self, [Create(loc, typeof(RuntimePointer<>).MakeGenericType(self))]) ?? throw new ArgumentException("Error dynamically creating a RuntimeObject");
    }

    public abstract class RuntimeObject<TProject, TSelf> : IBaseRuntimeObject, IRuntimeProperty<TSelf> where TProject : Project where TSelf : RuntimeObject<TProject, TSelf>
    {
        public IPointer<TSelf> Pointer { get; }
        public IPointer GetPointer() => Pointer;

        public TSelf PropValue { get => (TSelf)this; }

        public bool Freed { get; protected set; }

        public RuntimeProperty<NBTInt> ReferenceCount { get => new(GetProp<NBTInt>("__ref")); set => SetProp("__ref", value); }

        public RuntimeObject(IPointer<TSelf> loc)
        {
            Pointer = loc;
        }

        public RuntimeObject() { }

        public virtual (string, Type)[] AllProperties { get; }

        protected IPointer<T> GetProp<T>(string path, bool dot = true) where T : Pointerable => Pointer.Get<T>(path, dot);
        protected T GetObj<T>(string path, bool dot = true) where T : IBaseRuntimeObject => (T)T.Create((RuntimePointer<T>)RuntimePointer<T>.Create(Pointer.Get<RuntimePointer<T>>(path, dot)));

        protected void SetProp(string path, NBTType val) => Pointer.Get<NBTType>(path).Set(val);
        protected void SetProp<T>(string path, IPointer<T> pointer) where T : Pointerable
        {
            var place = GetProp<T>(path).Get<NBTString>("obj");
            if (typeof(T).IsAssignableTo(typeof(IBaseRuntimeObject))) ((IBaseRuntimeObject)pointer.Self).ReferenceCount.Pointer.With(i => i.Add(1));
            Project.ActiveProject.Std.StorePointer([.. place.StandardMacros([], "1"), .. pointer.StandardMacros([], "2")]);
        }

        protected void SetProp<T>(string path, IRuntimeProperty<T> prop) where T : Pointerable
        {
            if (prop.Pointer is not null) SetProp(path, prop.Pointer);
            else if (NBTType.IsNBTType<T>()) SetProp(path, NBTType.ToNBT(prop.PropValue ?? throw new ArgumentException("RuntimeProperty was not created properly")) ?? throw new Exception("How did we get here?"));
            else throw new ArgumentException("RuntimeProperty was not created properly");
        }

        public virtual void FreeObj()
        {
            FreePointers();
            Pointer.Free();
            Freed = true;
        }
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

        public virtual void FreePointers() { }

        private static TProject? _state = null;
        public static TProject State
        {
            get
            {
                _state ??= Project.Create<TProject>(Project.ActiveProject.Datapack);
                return _state;
            }
        }

        public static IBaseRuntimeObject Create<T>(IPointer<T> pointer) where T : Pointerable
        {
            return (IBaseRuntimeObject?)Activator.CreateInstance(typeof(TSelf), [pointer]) ?? throw new ArgumentException("Failed to create runtime object");
        }

        public static IRuntimeArgument Create(ScoreRef arg) => Create((IPointer<TSelf>)HeapPointer<TSelf>.Create(arg));

        public static implicit operator RuntimeObject<TProject, TSelf>(HeapPointer<TSelf> pointer) => (RuntimeObject<TProject, TSelf>)Create<TSelf>(pointer);

        public virtual IPointer ToPointer() => Pointer;
    }
}
