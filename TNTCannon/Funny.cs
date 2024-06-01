using System;
using Datapack.Net.CubeLib;

namespace TNTCannon
{
    public class Funny(HeapPointer<Funny> prop) : RuntimeObject<TNTProject, Funny>(prop)
    {
        // [MCProperty("value")]
        // public HeapPointer<int> Value {}
    }
}
