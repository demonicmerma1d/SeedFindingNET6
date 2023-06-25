using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFindingNET6
{
    public class BitEncoder
    {
        public Dictionary<string,int> EncodeOrder;
        public long EncodeData;
        public BitEncoder()
        {
            EncodeOrder = new Dictionary<string, int>();
            EncodeData = new long();
        }
        public BitEncoder(List<string> encodeOrder, long encodeData) //encoding
        {
            EncodeOrder = encodeOrder.Zip(Enumerable.Range(0,encodeOrder.Count)).ToDictionary(x => x.First, x => x.Second); //zips up a provided list into dictionary for quicker ref to index
            EncodeData = encodeData;
        }
        private long Get(string data)
        {
            long dataValue = 1 << EncodeOrder[data];
            return (dataValue & EncodeData);
        }
        private void Set(string data, bool value)
        {
            long dataValue = Get(data);
            if (!(dataValue == 0 == value)) EncodeData ^= dataValue;
        }
        public void Clear()
        {
            EncodeData = 0;
        }
        public void Remove(string data)
        {
            if (EncodeOrder.ContainsKey(data)) Set(data, false);
        }
        public bool TryGetValue(string data, out bool outValue)
        {
            outValue = false;
            if (!EncodeOrder.ContainsKey(data)) return false;
            outValue = GetValue(data);
            return true;
        }
        public bool GetValue(string data)
        {
            return Get(data) != 0;
        }
        public bool TrySetValue(string data, bool value)
        {
            if (!EncodeOrder.ContainsKey(data)) return false;
            Set(data, value);
            return true;
        }
        public bool Contains(HashSet<string> set)
        {
            long CompareValue = 0;
            foreach (var data in set) CompareValue ^= 1 << EncodeOrder[data];
            return Contains(CompareValue);
        }
        public bool Contains(long CompareData)
        {
            long dataValue = EncodeData & CompareData;
            return (dataValue^CompareData) == 0;
        }
        public bool ContainsInverse(long CompareData)
        {
            long dataValue = EncodeData & ~ CompareData;
            return (dataValue ^ CompareData) == long.MaxValue;
        }
        public bool ContainsInverse(HashSet<string> set)
        {
            long CompareValue = 0;
            foreach (var data in set) CompareValue ^= 1 << EncodeOrder[data];
            return ContainsInverse(CompareValue);
        }
        public bool ContainsSubset(HashSet<string> set) //contains a subset of set
        {
            long CompareValue = 0;
            foreach (var data in set) CompareValue ^= 1 << EncodeOrder[data];
            return (EncodeData & CompareValue) != 0;
        }
    }
}
