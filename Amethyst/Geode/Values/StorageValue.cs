using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.Values
{
	public class StorageValue(Storage storage, string path, TypeSpecifier type) : DataTargetValue
    {
        public readonly Storage Storage = storage;
        public readonly string Path = path;

		public override IDataTarget Target => new StorageTarget(Storage, Path);
        public override TypeSpecifier Type => type;

        public override DataTargetValue Property(string member, TypeSpecifier type) => new StorageValue(Storage, $"{Path}.{member}", type);
        public override DataTargetValue Index(int index, TypeSpecifier type) => new StorageValue(Storage, $"{Path}[{index}]", type);

        public override bool Equals(object? obj) => obj is StorageValue s && s.Storage == Storage && s.Path == Path;
        
        public override string ToString() => $"{Storage}.{Path}";
        public override int GetHashCode() => Storage.GetHashCode() * Path.GetHashCode() * Type.GetHashCode();
    }
}
