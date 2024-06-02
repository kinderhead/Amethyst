using System;
using Datapack.Net.CubeLib;

namespace TNTCannon
{
    public partial class Funny(HeapPointer<Funny> prop) : RuntimeObject<TNTProject, Funny>(prop)
    {
        public RuntimeProperty<int> Value { get => Get<int>("value"); set => Set("value", value); }

        [DeclareMC("say")]
        private static void _Say(Funny self)
        {
            State.Print(self.Value);
        }
    }
}
