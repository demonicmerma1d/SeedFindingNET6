using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFindingNET6
{
    public class Utility
    {
        public static Dictionary<int, int> DictSum(Dictionary<int, int> Dict1, Dictionary<int, int> Dict2)
        {
            foreach (KeyValuePair<int, int> Pair in Dict2)
            {
                if (Dict1.TryAdd(Pair.Key, Pair.Value)) continue;
                Dict1[Pair.Key] += Pair.Value;
            }
            return Dict1;
        }
        public static HashSet<int> BundleIntersect(HashSet<int> Hash1,HashSet<int> Hash2)
        {
            Hash1.IntersectWith(Hash2);
            return Hash1;
        }
    }
}
