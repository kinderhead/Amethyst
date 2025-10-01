namespace Datapack.Net.Function
{
    public readonly struct Score(string name, string criteria, string displayName = "") : IComparable<Score>
    {
        public readonly string Name = name;
        public readonly string Criteria = criteria;
        public readonly string DisplayName = displayName;

        public override string ToString()
        {
            return Name;
        }

        public static bool operator ==(Score? a, Score? b) => a?.Name == b?.Name;
        public static bool operator !=(Score? a, Score? b) => a?.Name != b?.Name;

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is Score s) return this == s;
            return obj.Equals(this);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public int CompareTo(Score other) => Name.CompareTo(other.Name);
    }
}
