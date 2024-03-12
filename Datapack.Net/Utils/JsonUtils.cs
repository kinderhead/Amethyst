using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Utils
{
    public static class JsonUtils
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new NamespacedIDSerializer());
        }
    }
}
