using System;
using System.Collections.Generic;
using System.Text;

namespace Datapack.Net.Function
{
	public interface IDataTarget
	{
		public string? Path { get; }
		public string GetTarget();
	}

	public readonly struct StorageTarget(Storage storage, string? path) : IDataTarget
	{
		public readonly Storage Storage = storage;
        public string? Path => path;

		public string GetTarget() => $"storage {Storage}{(Path is null ? "" : " " + Path)}";
		public override string ToString() => GetTarget();
    }

    public readonly struct BlockDataTarget(Position position, string? path) : IDataTarget
    {
        public readonly Position Position = position;
        public string? Path => path;

        public string GetTarget() => $"block {Position}{(Path is null ? "" : " " + Path)}";
        public override string ToString() => GetTarget();
    }

    public readonly struct EntityDataTarget(IEntityTarget target, string? path) : IDataTarget
    {
        public readonly IEntityTarget Target = target.RequireOne();
        public string? Path => path;

        public string GetTarget() => $"entity {Target.Get()}{(Path is null ? "" : " " + Path)}";
        public override string ToString() => GetTarget();
    }
}
