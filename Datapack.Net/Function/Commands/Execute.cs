using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function.Commands
{
    public class Execute(bool macro = false) : Command(macro)
    {
        public readonly List<Subcommand> Subcommands = [];

        protected override string PreBuild()
        {
            StringBuilder sb = new("execute ");

            bool run = false;
            foreach (var i in Subcommands)
            {
                sb.Append(i.ToString());
                sb.Append(' ');

                if (i is Subcommand.Run)
                {
                    run = true;
                    break;
                }
            }

            if (!run) throw new ArgumentException("Execute command does not contain a run subcommand.");

            return sb.ToString().TrimEnd();
        }

        public Execute Run(Command cmd) => Add(new Subcommand.Run(cmd));
        public Execute Align(Swizzle axes) => Add(new Subcommand.Align(axes));
        public Execute Anchored(bool eyes) => Add(new Subcommand.Anchored(eyes));
        public Execute As(IEntityTarget target) => Add(new Subcommand.As(target));
        public Execute At(IEntityTarget target) => Add(new Subcommand.At(target));
        public Execute Facing(Position pos) => Add(new Subcommand.Facing(pos));
        public Execute Facing(IEntityTarget target, bool eyes = false) => Add(new Subcommand.Facing(target, eyes));
        public Execute In(Dimension dimension) => Add(new Subcommand.In(dimension));
        public Execute On(OnRelation relation) => Add(new Subcommand.On(relation));
        public Execute Positioned(Position pos) => Add(new Subcommand.Positioned(pos));
        public Execute Positioned(IEntityTarget target) => Add(new Subcommand.Positioned(target));
        public Execute Positioned(Heightmap heightmap) => Add(new Subcommand.Positioned(heightmap));
        public Execute Rotated(Rotation rotation) => Add(new Subcommand.Rotated(rotation));
        public Execute Rotated(IEntityTarget target) => Add(new Subcommand.Rotated(target));
        public Execute Summon(IEntityTarget target) => Add(new Subcommand.Summon(target));

        public Execute Add(Subcommand sbc)
        {
            Subcommands.Add(sbc);
            return this;
        }

        public class Conditional(Execute execute, Conditional.Type type)
        {
            private readonly Execute Execute = execute;
            public readonly Type ConditionalType = type;

            public Execute Add(Subcommand sbc)
            {
                sbc.Type = ConditionalType;
                return Execute.Add(sbc);
            }

            public Execute Biome(Position pos, Biome biome) => Add(new Subcommand.Biome(pos, biome));
            public Execute Block(Position pos, Block block) => Add(new Subcommand.Block(pos, block));
            public Execute Blocks(Position start, Position end, Position destination, bool masked = false) => Add(new Subcommand.Blocks(start, end, destination, masked));

            public enum Type
            {
                If,
                Unless
            }

            public abstract class Subcommand : Execute.Subcommand
            {
                public Type Type;

                public abstract string Get();

                public override string ToString()
                {
                    return $"{Enum.GetName(Type)?.ToLower()} {Get()}";
                }

                public class Biome(Position position, Net.Data.Biome biome) : Subcommand
                {
                    public readonly Position Position = position;
                    public readonly Net.Data.Biome BiomeTag = biome;

                    public override string Get()
                    {
                        return $"biome {Position} {BiomeTag}";
                    }
                }

                public class Block(Position position, Net.Data.Block block) : Subcommand
                {
                    public readonly Position Position = position;
                    public readonly Net.Data.Block BlockTag = block;

                    public override string Get()
                    {
                        return $"block {Position} {BlockTag}";
                    }
                }

                public class Blocks(Position start, Position end, Position destination, bool masked) : Subcommand
                {
                    public readonly Position Start = start;
                    public readonly Position End = end;
                    public readonly Position Destination = destination;
                    public readonly bool Masked = masked;

                    public override string Get()
                    {
                        return $"blocks {Start} {End} {Destination} {(Masked ? "masked" : "all")}";
                    }
                }

                public class Data : Subcommand
                {
                    public readonly Position? Position;
                    public readonly IEntityTarget? Target;
                    public readonly Storage? Source;

                    public override string Get()
                    {
                        return $"data ";
                    }
                }
            }
        }

        public class Subcommand
        {
            public class Run(Command cmd) : Subcommand
            {
                public readonly Command Command = cmd;

                public override string ToString()
                {
                    return $"run {Command.Build()}";
                }
            }

            public class Align(Swizzle axes) : Subcommand
            {
                public readonly Swizzle Axes = axes;

                public override string ToString()
                {
                    return $"align {Axes}";
                }
            }

            public class Anchored(bool eyes) : Subcommand
            {
                public readonly bool Eyes = eyes;

                public override string ToString()
                {
                    return $"anchored {(Eyes ? "eyes" : "feet")}";
                }
            }

            public class As(IEntityTarget target) : Subcommand
            {
                public readonly IEntityTarget Target = target;

                public override string ToString()
                {
                    return $"as {Target.Get()}";
                }
            }

            public class At(IEntityTarget target) : Subcommand
            {
                public readonly IEntityTarget Target = target;

                public override string ToString()
                {
                    return $"at {Target.Get()}";
                }
            }

            public class Facing : Subcommand
            {
                public readonly Position? Position;
                public readonly IEntityTarget? Target;
                public readonly bool Eyes;

                public Facing(Position pos)
                {
                    Position = pos;
                }

                public Facing(IEntityTarget target, bool eyes = false)
                {
                    Target = target;
                    Eyes = eyes;
                }

                public override string ToString()
                {
                    if (Position != null)
                    {
                        return $"facing {Position}";
                    }
                    else
                    {
                        return $"facing {Target?.Get()} {(Eyes ? "eyes" : "feet")}";
                    }
                }
            }

            public class In(Dimension dimension) : Subcommand
            {
                public readonly Dimension Dimension = dimension;

                public override string ToString()
                {
                    return $"in {Dimension}";
                }
            }

            public class On(OnRelation relation) : Subcommand
            {
                public readonly OnRelation Relation = relation;

                public override string ToString()
                {
                    return $"on {Enum.GetName(Relation)?.ToLower()}";
                }
            }

            public class Positioned : Subcommand
            {
                public readonly Position? Position;
                public readonly IEntityTarget? Target;
                public readonly Heightmap Heightmap;

                public Positioned(Position pos)
                {
                    Position = pos;
                }

                public Positioned(IEntityTarget target)
                {
                    Target = target;
                }

                public Positioned(Heightmap heightmap)
                {
                    Heightmap = heightmap;
                }

                public override string ToString()
                {
                    if (Position != null)
                    {
                        return $"positioned {Position}";
                    }
                    else if (Target != null)
                    {
                        return $"positioned as {Target?.Get()}";
                    }
                    else
                    {
                        return $"positioned over {Enum.GetName(Heightmap)?.ToLower()}";
                    }
                }
            }

            public class Rotated : Subcommand
            {
                public readonly Rotation? Rotation;
                public readonly IEntityTarget? Target;

                public Rotated(Rotation rot)
                {
                    Rotation = rot;
                }

                public Rotated(IEntityTarget target)
                {
                    Target = target;
                }

                public override string ToString()
                {
                    if (Rotation != null)
                    {
                        return $"rotated {Rotation}";
                    }
                    else
                    {
                        return $"rotated as {Target?.Get()}";
                    }
                }
            }

            public class Summon(IEntityTarget target) : Subcommand
            {
                public readonly IEntityTarget Target = target;

                public override string ToString()
                {
                    return $"summon {Target.Get()}";
                }
            }
        }
    }

    public enum OnRelation
    {
        Attacker,
        Controller,
        Leasher,
        Origin,
        Owner,
        Passengers,
        Target,
        Vehicle
    }

    public enum Heightmap
    {
        World_Surface,
        Motion_Blocking,
        Motion_Blocking_No_Leaves,
        Ocean_Floor
    }
}
