data = """public readonly double? X;
public readonly double? Y;
public readonly double? Z;
public readonly MCRange<float>? Distance;
public readonly double? Dx;
public readonly double? Dy;
public readonly double? Dz;
public readonly Dictionary<Score, MCRange<int>>? Scores;
public readonly List<Negatable<EntityTag>>? Tag;
public readonly Negatable<Team>? Team;
public readonly SortType? Sort;
public readonly int? Limit;
public readonly MCRange<int>? Level;
public readonly Gamemode? Gamemode;
public readonly string? Name;
public readonly MCRange<float>? X_rotation;
public readonly MCRange<float>? Y_rotation;
public readonly EntityTypeSelector? Type;
public readonly Negatable<NBTCompound>? NBT;"""

args = [tuple(i[16:].strip(";").split(" ")) for i in data.splitlines()]

print(", ".join([f"{i[0]} {i[1].lower()} = null" for i in args]))
