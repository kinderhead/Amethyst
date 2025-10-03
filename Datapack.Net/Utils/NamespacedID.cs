using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Datapack.Net.Utils
{
	public readonly partial struct NamespacedID
	{
		public readonly string Namespace;
		public readonly string Path;

		public NamespacedID(string @namespace, string path) : this($"{@namespace}{(@namespace.Contains(':') ? '/' : ':')}{path}") { }

		public NamespacedID(string id)
		{
			if (id.Count(c => c == ':') != 1)
			{
				throw new FormatException($"Invalid namespaced id: {id}");
			}

			var parts = id.Split(":");
			Namespace = parts[0];
			Path = DuplicateSlashRegex().Replace(parts[1], "/");
		}

		public override string ToString() => $"{Namespace}:{Path}";

		public static bool operator ==(NamespacedID left, NamespacedID right) => left.Namespace == right.Namespace && left.Path == right.Path;
		public static bool operator !=(NamespacedID left, NamespacedID right) => left.Namespace != right.Namespace || left.Path != right.Path;

		public static implicit operator NamespacedID(string id) => new(id);

		public override bool Equals(object? obj)
		{
			if (obj is NamespacedID id)
			{
				return this == id;
			}

			return base.Equals(obj);
		}

		public override int GetHashCode() => Namespace.GetHashCode() * Path.GetHashCode();

		[GeneratedRegex(@"/+")]
		private static partial Regex DuplicateSlashRegex();

		public string ContainingFolder()
		{
			if (Path.Contains('/'))
			{
				return $"{Namespace}:{string.Join('/', Path.Split('/')[..^1])}";
			}
			else
			{
				return Namespace;
			}
		}
	}

	public class NamespacedIDSerializer : JsonConverter<NamespacedID>
	{
		public override NamespacedID ReadJson(JsonReader reader, Type objectType, NamespacedID existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.Value is string s)
			{
				return new(s);
			}

			throw new JsonReaderException("Invalid format for namespaced id");
		}

		public override void WriteJson(JsonWriter writer, NamespacedID value, JsonSerializer serializer) => writer.WriteValue(value.ToString());
	}
}
