using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public class TargetSelector(TargetType targetType, double? x = null, double? y = null, double? z = null, MCRange<float>? distance = null, double? dx = null, double? dy = null, double? dz = null, Dictionary<Score, MCRange<int>>? scores = null, List<Negatable<EntityTag>>? tag = null, Negatable<Team>? team = null, SortType? sort = null, int? limit = null, MCRange<int>? level = null, Gamemode? gamemode = null, string? name = null, MCRange<float>? x_rotation = null, MCRange<float>? y_rotation = null, EntityTypeSelector? type = null, Negatable<NBTCompound>? nbt = null) : IEntityTarget
    {
        public readonly TargetType TargetType = targetType;
        public readonly double? X = x;
        public readonly double? Y = y;
        public readonly double? Z = z;
        public readonly MCRange<float>? Distance = distance;
        public readonly double? Dx = dx;
        public readonly double? Dy = dy;
        public readonly double? Dz = dz;
        public readonly Dictionary<Score, MCRange<int>>? Scores = scores;
        public readonly List<Negatable<EntityTag>>? Tag = tag;
        public readonly Negatable<Team>? Team = team;
        public readonly SortType? Sort = sort;
        public readonly int? Limit = limit;
        public readonly MCRange<int>? Level = level;
        public readonly Gamemode? Gamemode = gamemode;
        public readonly string? Name = name;
        public readonly MCRange<float>? X_rotation = x_rotation;
        public readonly MCRange<float>? Y_rotation = y_rotation;
        public readonly EntityTypeSelector? Type = type;
        public readonly Negatable<NBTCompound>? NBT = nbt;

        public string Get()
        {
            List<KeyValuePair<string, string>> args = [];

            if (X != null) args.Add(new("x", $"{X}"));
            if (Y != null) args.Add(new("y", $"{Y}"));
            if (Z != null) args.Add(new("z", $"{Z}"));
            if (Distance != null) args.Add(new("distance", $"{Distance}"));
            if (Dx != null) args.Add(new("dx", $"{Dx}"));
            if (Dy != null) args.Add(new("dy", $"{Dy}"));
            if (Dz != null) args.Add(new("dz", $"{Dz}"));
            if (Scores != null) args.Add(new("scores", $"{{{CompileDict(Scores)}}}"));

            if (Tag != null)
            {
                foreach (var i in Tag)
                {
                    args.Add(new("tag", i.ToString()));
                }
                
                if (Tag.Count == 0) args.Add(new("tag", ""));
            }

            if (Team != null) args.Add(new("team", Team.ToString()));
            if (Sort != null) args.Add(new("sort", $"{Enum.GetName((SortType)Sort)?.ToLower()}"));
            if (Limit != null) args.Add(new("limit", $"{Limit}"));
            if (Level != null) args.Add(new("level", $"{Level}"));
            if (Gamemode != null) args.Add(new("gamemode", $"{Enum.GetName((Gamemode)Gamemode)?.ToLower()}"));
            if (Name != null) args.Add(new("name", $"{Name}"));
            if (X_rotation != null) args.Add(new("x_rotation", $"{X_rotation}"));
            if (Y_rotation != null) args.Add(new("y_rotation", $"{Y_rotation}"));
            
            if (Type != null)
            {
                var nonNegated = false;
                foreach (var i in Type)
                {
                    if (!i.Negative) nonNegated = true;
                    args.Add(new("type", i.ToString()));
                }
                if (nonNegated && Type.Count >= 2) throw new ArgumentException("Type parameter is invalid. It contains a negated and non negated values", "type");
            }

            if (NBT != null) args.Add(new("nbt", $"{NBT}"));

            if (args.Count > 0)
            {
                return $"{GetTypeName(TargetType)}[{CompileDict(args)}]";
            }

            return GetTypeName(TargetType);
        }

        public static string GetTypeName(TargetType type)
        {
            var s = Enum.GetName(type) ?? throw new MissingFieldException($"Missing type {type}");
            return $"@{s.ToLower()}";
        }

        public static string CompileDict<T, E>(ICollection<KeyValuePair<T, E>> dict) where T : notnull where E : notnull
        {
            StringBuilder sb = new();
            foreach (var i in dict)
            {
                sb.Append(i.Key.ToString());
                sb.Append('=');
                sb.Append(i.Value.ToString());
                sb.Append(',');
            }
            if (sb.Length > 0) sb.Length--;
            return sb.ToString();
        }

        public bool IsOne()
        {
            if (TargetType == TargetType.a || TargetType == TargetType.e && Limit == 1)
            {
                return true;
            }
            else if (TargetType == TargetType.s && Limit == null)
            {
                return true;
            }
            else if (TargetType == TargetType.p || TargetType == TargetType.r && (Limit == null || Limit == 1))
            {
                return true;
            }

            return false;
        }
    }

    public enum TargetType
    {
        p,
        r,
        a,
        e,
        s
    }

    public enum SortType
    {
        Nearest,
        Furtherst,
        Random,
        Arbitrary
    }

    public class EntityTypeSelector : List<Negatable<EntityType>>
    {
        public static implicit operator EntityTypeSelector(Negatable<EntityType> type)
        {
            return [type];
        }

        public static implicit operator EntityTypeSelector(EntityType type)
        {
            return [type];
        }
    }
}
