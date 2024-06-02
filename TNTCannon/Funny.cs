using System;
using Datapack.Net.CubeLib;

namespace TNTCannon
{
    [RuntimeObject("funny")]
    public partial class Funny(HeapPointer<Funny> prop) : RuntimeObject<TNTProject, Funny>(prop)
    {
        public RuntimeProperty<int> Value { get => Get<int>("value"); set => Set("value", value); }
        public RuntimeProperty<string> Str { get => Get<string>("str"); set => Set("str", value); }

        [DeclareMC("say")]
        private static void _Say(Funny self)
        {
            State.Print(self.Value);
        }
    }
}
