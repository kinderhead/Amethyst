using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Utils
{
    public readonly struct NamespacedID(string @namespace, string path)
    {
        public readonly string Namespace = @namespace;
        public readonly string Path = path;

        public NamespacedID(string id) : this(id.Split(":")[0], id.Split(":")[1])
        {
            if (!id.Contains(':'))
            {
                throw new FormatException($"Invalid namespaced id: {id}");
            }
        }

        public override string ToString()
        {
            return $"{Namespace}:{Path}";
        }

        public static bool operator ==(NamespacedID left, NamespacedID right) => left.Namespace == right.Namespace && left.Path == right.Path;
        public static bool operator !=(NamespacedID left, NamespacedID right) => left.Namespace != right.Namespace || left.Path != right.Path;

        public override bool Equals(object? obj)
        {
            if (obj is NamespacedID id)
            {
                return this == id;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Namespace.GetHashCode() * Path.GetHashCode();
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

        public override void WriteJson(JsonWriter writer, NamespacedID value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
