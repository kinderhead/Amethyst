using System.Diagnostics.CodeAnalysis;
using System.Text;
using Datapack.Net.Data;

// ReSharper disable NotResolvedInText

namespace Datapack.Net.Function
{
    // Not struct because this is chonky
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
    public class TargetSelector(
        TargetType targetType,
        double? x = null,
        double? y = null,
        double? z = null,
        MCRange<float>? distance = null,
        double? dx = null,
        double? dy = null,
        double? dz = null,
        Dictionary<Score, MCRange<int>>? scores = null,
        List<Negatable<EntityTag>>? tag = null,
        Negatable<Team>? team = null,
        SortType? sort = null,
        int? limit = null,
        MCRange<int>? level = null,
        Gamemode? gamemode = null,
        string? name = null,
        MCRange<float>? x_rotation = null,
        MCRange<float>? y_rotation = null,
        EntityTypeSelector? type = null,
        Negatable<NBTCompound>? nbt = null) : IEntityTarget
    {
        public readonly MCRange<float>? Distance = distance;
        public readonly double? Dx = dx;
        public readonly double? Dy = dy;
        public readonly double? Dz = dz;
        public readonly Gamemode? Gamemode = gamemode;
        public readonly MCRange<int>? Level = level;
        public readonly int? Limit = limit;
        public readonly string? Name = name;
        public readonly Negatable<NBTCompound>? NBT = nbt;
        public readonly Dictionary<Score, MCRange<int>>? Scores = scores;
        public readonly SortType? Sort = sort;
        public readonly List<Negatable<EntityTag>>? Tag = tag;
        public readonly TargetType TargetType = targetType;
        public readonly Negatable<Team>? Team = team;
        public readonly EntityTypeSelector? Type = type;
        public readonly double? X = x;
        public readonly MCRange<float>? XRotation = x_rotation;
        public readonly double? Y = y;
        public readonly MCRange<float>? YRotation = y_rotation;
        public readonly double? Z = z;

        public static TargetSelector Self => new(TargetType.s);

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

            if (XRotation != null) args.Add(new("x_rotation", $"{XRotation}"));

            if (YRotation != null) args.Add(new("y_rotation", $"{YRotation}"));

            if (Type != null)
            {
                var nonNegated = false;
                foreach (var i in Type)
                {
                    if (!i.Negative) nonNegated = true;

                    args.Add(new("type", i.ToString()));
                }

                if (nonNegated && Type.Count >= 2)
                {
                    throw new ArgumentException(
                        "Type parameter is invalid. It contains a negated and non negated values", "type");
                }
            }

            if (NBT != null) args.Add(new("nbt", $"{NBT}"));

            if (args.Count > 0) return $"{GetTypeName(TargetType)}[{CompileDict(args)}]";

            return GetTypeName(TargetType);
        }

        public bool IsOne()
        {
            if (TargetType == TargetType.a || (TargetType == TargetType.e && Limit == 1)) return true;

            if (TargetType == TargetType.s && Limit == null) return true;

            if (TargetType == TargetType.p || TargetType == TargetType.n ||
                (TargetType == TargetType.r && (Limit == null || Limit == 1)))
                return true;

            return false;
        }

        public static string GetTypeName(TargetType type)
        {
            var s = Enum.GetName(type) ?? throw new MissingFieldException($"Missing type {type}");
            return $"@{s.ToLower()}";
        }

        public static string CompileDict<TKey, TVal>(ICollection<KeyValuePair<TKey, TVal>> dict) where TKey : notnull where TVal : notnull
        {
            StringBuilder sb = new();
            foreach (var i in dict)
            {
                sb.Append(i.Key);
                sb.Append('=');
                sb.Append(i.Value);
                sb.Append(',');
            }

            if (sb.Length > 0) sb.Length--;

            return sb.ToString();
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum TargetType
    {
        p,
        r,
        a,
        e,
        s,
        n
    }

    public enum SortType
    {
        Nearest,
        Furtherst,
        Random,
        Arbitrary
    }

    public class EntityTypeSelector : List<Negatable<EntityData>>
    {
        public static implicit operator EntityTypeSelector(Negatable<EntityData> type) => [type];

        public static implicit operator EntityTypeSelector(EntityData type) => [type];
    }
}