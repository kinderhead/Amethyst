using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function.Commands
{
    public class Execute : Command, ICloneable
    {
        public readonly List<Subcommand> Subcommands = [];

        public readonly Conditional If;
        public readonly Conditional Unless;

        public Execute(bool macro = false) : base(macro)
        {
            If = new(this, Conditional.Type.If);
            Unless = new(this, Conditional.Type.Unless);
        }

        protected override string PreBuild()
        {
            StringBuilder sb = new("execute ");

            Subcommand.Run? run = null;
            foreach (var i in Subcommands)
            {
                if (i is Subcommand.Run r)
                {
                    if (run is not null) throw new ArgumentException($"Execute command (incomplete) {sb} has multiple runs");
                    run = r;
                    continue;
                }

                sb.Append(i.ToString());
                sb.Append(' ');
            }

            if (run is null) throw new ArgumentException($"Execute command {sb} does not have a run");
            else sb.Append(run.ToString());

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
        public Execute Summon(EntityType target) => Add(new Subcommand.Summon(target));
        public Execute Store(Position pos, string path, NBTNumberType type, double scale, bool result = true) => Add(new Subcommand.Store { TargetPos = pos, Path = path, DataType = type, Scale = scale, Result = result });
        public Execute Store(Bossbar id, BossbarValueType type, bool result = true) => Add(new Subcommand.Store { BossbarID = id, BossbarType = type, Result = result });
        public Execute Store(IEntityTarget target, string path, NBTNumberType type, double scale, bool result = true) => Add(new Subcommand.Store { Target = target, Path = path, DataType = type, Scale = scale, Result = result });
        public Execute Store(IEntityTarget target, Score objective, bool result = true) => Add(new Subcommand.Store { Target = target, Objective = objective, Result = result });
        public Execute Store(Storage target, string path, NBTNumberType type, double scale, bool result = true) => Add(new Subcommand.Store { StorageTarget = target, Path = path, DataType = type, Scale = scale, Result = result });

        public Execute Add(Subcommand sbc)
        {
            Subcommands.Add(sbc);
            return this;
        }

        public T Get<T>() where T : Subcommand
        {
            foreach (var i in Subcommands) if (i is T) return (T)i;
            throw new Exception("Execute command does not have the requested subcommand");
        }

        public IEnumerable<T> GetAll<T>() where T : Subcommand
        {
            return Subcommands.Where(i => i is T).Cast<T>();
        }

        public void RemoveAll<T>() where T : Subcommand
        {
            Subcommands.RemoveAll((i) => i is T);
        }

        public bool Contains<T>() where T : Subcommand
        {
            foreach (var i in Subcommands)
            {
                if (i is T) return true;
            }

            return false;
        }

        public object Clone()
        {
            var other = new Execute(Macro);
            foreach (var i in Subcommands)
            {
                other.Subcommands.Add((Subcommand)i.Clone());
            }
            return other;
        }

        public Execute Copy() => (Execute)Clone();

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
            public Execute Data(Position pos, string path) => Add(new Subcommand.Data(pos, path));
            public Execute Data(IEntityTarget target, string path) => Add(new Subcommand.Data(target, path));
            public Execute Data(Storage source, string path) => Add(new Subcommand.Data(source, path));
            public Execute Dimension(Dimension dimension) => Add(new Subcommand.Dimension(dimension));
            public Execute Entity(IEntityTarget entities) => Add(new Subcommand.Entity(entities));
            public Execute Function(MCFunction function) => Add(new Subcommand.Function(function));
            public Execute Loaded(Position pos) => Add(new Subcommand.Loaded(pos));
            public Execute Score(IEntityTarget target, Score targetObjective, Comparison op, IEntityTarget source, Score sourceObjective) => Add(new Subcommand.Score(target, targetObjective, op, source, sourceObjective));
            public Execute Score(IEntityTarget target, Score targetObjective, MCRange<int> range) => Add(new Subcommand.Score(target, targetObjective, range));

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
                    public readonly string Path;

                    public Data(Position pos, string path)
                    {
                        Position = pos;
                        Path = path;
                    }

                    public Data(IEntityTarget target, string path)
                    {
                        Target = target;
                        Path = path;
                    }

                    public Data(Storage source, string path)
                    {
                        Source = source;
                        Path = path;
                    }

                    public override string Get()
                    {
                        if (Position != null)
                        {
                            return $"data block {Position} {Path}";
                        }
                        else if (Target != null)
                        {
                            return $"data entity {Target.Get()} {Path}";
                        }
                        else
                        {
                            return $"data storage {Source} {Path}";
                        }
                    }
                }

                public class Dimension(Net.Data.Dimension dimension) : Subcommand
                {
                    public readonly Net.Data.Dimension DimensionTag = dimension;

                    public override string Get()
                    {
                        return $"dimension {DimensionTag}";
                    }
                }

                public class Entity(IEntityTarget entities) : Subcommand
                {
                    public readonly IEntityTarget Entities = entities;

                    public override string Get()
                    {
                        return $"entity {Entities.Get()}";
                    }
                }

                public class Function(MCFunction function) : Subcommand
                {
                    public readonly MCFunction MCFunction = function;

                    public override string Get()
                    {
                        return $"function {MCFunction.ID}";
                    }
                }

                public class Loaded(Position pos) : Subcommand
                {
                    public readonly Position Position = pos;

                    public override string Get()
                    {
                        return $"loaded {Position}";
                    }
                }

                public class Score : Subcommand
                {
                    public readonly IEntityTarget Target;
                    public readonly Net.Function.Score TargetObjective;
                    public readonly Comparison? Operator;
                    public readonly IEntityTarget? Source;
                    public readonly Net.Function.Score? SourceObjective;
                    public readonly MCRange<int>? Range;

                    public Score(IEntityTarget target, Net.Function.Score targetObjective, Comparison op, IEntityTarget source, Net.Function.Score sourceObjective)
                    {
                        Target = target;
                        TargetObjective = targetObjective;
                        Operator = op;
                        Source = source;
                        SourceObjective = sourceObjective;
                    }

                    public Score(IEntityTarget target, Net.Function.Score targetObjective, MCRange<int> range)
                    {
                        Target = target;
                        TargetObjective = targetObjective;
                        Range = range;
                    }

                    public override string Get()
                    {
                        if (Operator != null)
                        {
                            return $"score {Target.Get()} {TargetObjective} {GetOperator((Comparison) Operator)} {Source?.Get()} {SourceObjective}";
                        }
                        else
                        {
                            return $"score {Target.Get()} {TargetObjective} matches {Range}";
                        }
                    }

                    public static string GetOperator(Comparison op)
                    {
                        switch (op)
                        {
                            case Comparison.LessThan:
                                return "<";
                            case Comparison.GreaterThan:
                                return ">";
                            case Comparison.LessThanOrEqual:
                                return "<=";
                            case Comparison.GreaterThanOrEqual:
                                return ">=";
                            case Comparison.Equal:
                                return "=";
                            default:
                                return "";
                        }
                    }
                }
            }
        }

        public class Subcommand : ICloneable
        {
            public object Clone()
            {
                return MemberwiseClone();
            }

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

            public class Summon(EntityType target) : Subcommand
            {
                public readonly EntityType Target = target;

                public override string ToString()
                {
                    return $"summon {Target}";
                }
            }

            public class Store : Subcommand
            {
                public bool Result;

                public Position? TargetPos;
                public IEntityTarget? Target;
                public Storage? StorageTarget;
                public string? Path;
                public NBTNumberType DataType;
                public double? Scale;

                public Bossbar? BossbarID;
                public BossbarValueType BossbarType;

                public Score? Objective;

                public override string ToString()
                {
                    var prefix = $"store {(Result ? "result" : "success")} ";
                    var postfix = "";

                    if (TargetPos != null)
                    {
                        postfix = $"block {TargetPos} {Path} {Enum.GetName(DataType)?.ToLower()} {Scale}";
                    }
                    else if (BossbarID != null)
                    {
                        postfix = $"bossbar {BossbarID} {Enum.GetName(BossbarType)?.ToLower()}";
                    }
                    else if (Objective != null)
                    {
                        postfix = $"score {Target?.Get()} {Objective}";
                    }
                    else if (Target != null)
                    {
                        postfix = $"entity {Target.Get()} {Path} {Enum.GetName(DataType)?.ToLower()} {Scale}";
                    }
                    else if (StorageTarget != null)
                    {
                        postfix = $"storage {StorageTarget} {Path} {Enum.GetName(DataType)?.ToLower()} {Scale}";
                    }

                    return prefix + postfix;
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

    public enum Comparison
    {
        Equal,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }
}
