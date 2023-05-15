using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace SeedFindingNET6
{
    public class ObjectInfo
    {
        public struct ObjectData
        {
            public string Name;
            public int Cost;
            public int Id;
            public ObjectData(int id, string name, int cost) { Id = id; Name = name; Cost = cost; }
            public override string ToString()
            {
                return string.Format("[{0:D3} {1}]", Id, Name);
            }
        }
        public static Dictionary<int, ObjectData> Items;
        static ObjectInfo()
        {
            Items = new Dictionary<int, ObjectData>();
            Console.WriteLine(File.Exists(@"data/object_info.json"));
            Dictionary<string, string> items = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"data/object_info.json"));
            foreach (var kvp in items)
            {
                int id = Int32.Parse(kvp.Key);
                string[] tokens = kvp.Value.Split("/");
                Items.Add(id, new ObjectData(id, tokens[0], Int32.Parse(tokens[1])));
            }
        }

        public static ObjectData Get(int id)
        {
            return Items[id];
        }
    }
}
