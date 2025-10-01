namespace Datapack.Net.Function
{
    public interface IDataTarget
    {
        public string? Path { get; }
        public string Type { get; }
        public string Source { get; }

        public string GetTarget();
    }

    public readonly struct StorageTarget(Storage storage, string? path) : IDataTarget
    {
        public readonly Storage Storage = storage;
        public string? Path => path;

		public string Type => "storage";
        public string Source => Storage.ToString();

		public string GetTarget() => $"storage {Storage}{(Path is null ? "" : " " + Path)}";
        public override string ToString() => GetTarget();
    }

    public readonly struct BlockDataTarget(Position position, string? path) : IDataTarget
    {
        public readonly Position Position = position;
        public string? Path => path;

        public string Type => "block";
        public string Source => Position.ToString();

        public string GetTarget() => $"block {Position}{(Path is null ? "" : " " + Path)}";
        public override string ToString() => GetTarget();
    }

    public readonly struct EntityDataTarget(IEntityTarget target, string? path) : IDataTarget
    {
        public readonly IEntityTarget Target = target.RequireOne();
        public string? Path => path;

        public string Type => "entity";
        public string Source => Target.Get();

        public string GetTarget() => $"entity {Target.Get()}{(Path is null ? "" : " " + Path)}";
        public override string ToString() => GetTarget();
    }

    public readonly struct RawDataTarget(string target) : IDataTarget
    {
        public readonly string Target = target;

        public string Type => Target.Split(' ')[0];
        public string Source => Target.Split(' ')[1];
        public string? Path => Target.Split(' ')[2];

        public string GetTarget() => Target ?? "";
        public override string ToString() => GetTarget();
    }
}
