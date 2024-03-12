using Datapack.Net.Function;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public class Block(NamespacedID id)
    {
        public readonly NamespacedID ID = id;

        public override string ToString()
        {
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            if (fields.Length == 0) return ID.ToString();
            else
            {
                Dictionary<string, string> values = [];
                foreach (var i in fields)
                {
                    object? value = i.GetValue(this);
                    if (value == null) continue;
                    if (i.Name == "ID") continue;

                    values.Add(i.Name.ToLower(), $"{value}".ToLower());
                }
                return $"{ID}[{TargetSelector.CompileDict(values)}]";
            }
        }
    }
}
