using System;

namespace Datapack.Net.CubeLib
{
    public abstract class BaseRuntimeObject() {}

    public abstract class RuntimeObject<TProject, TSelf>(HeapPointer<TSelf> loc) : BaseRuntimeObject where TProject : Project
    {
        public readonly HeapPointer<TSelf> Location = loc;

        public static TProject State => (TProject) Project.ActiveProject;
    }
}
