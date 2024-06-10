using System;
using Datapack.Net.CubeLib;

namespace TNTCannon
{
    [RuntimeObject("funny")]
    public partial class Funny(IPointer<Funny> prop) : RuntimeObject<TNTProject, Funny>(prop)
    {
        internal sealed class Props
        {
            [RuntimeProperty("prop")]
            public int Prop { get; set; }

            [RuntimeProperty("str")]
            public string Str { get; set; }

            [RuntimeProperty("other")]
            public Funny Other { get; set; }
        }

        [DeclareMC("say")]
        private static void _Say(Funny self)
        {
            State.Print(self.Prop);
        }
    }
}
