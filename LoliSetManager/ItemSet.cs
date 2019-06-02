using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoliSetManager

{
    public class ItemSet
    {
        public string title;
        public enum Type { custom, global }
        [JsonConverter(typeof(StringEnumConverter))]
        public Type type;
        public enum Map { any, SR, HA, TT, CS }
        [JsonConverter(typeof(StringEnumConverter))]
        public Map map;
        public enum Mode { any, CLASSIC, ARAM, ODIM }
        [JsonConverter(typeof(StringEnumConverter))]
        public Mode mode;
        public bool priority;
        public uint sortrank;
        public class Block
        {
            public string type;
            public bool recMath;
            public int minSummonerLevel = -1;
            public int maxSummonerLevel = -1;
            public string showIfSummonerSpell;
            public string hideIfSummonerSpell;
            public class Item
            {
                public string id;
                public int count = 1;
                public static implicit operator Item(string id) => new Item() { id = id };

                public static implicit operator Item(Dictionary<string, object> v)
                {
                    return JsonConvert.DeserializeObject<Item>(JsonConvert.SerializeObject(v));
                }
                public static implicit operator Dictionary<string, object>(Item v)
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(v));
                }
            }
            public Item[] items;

            public static implicit operator Block(Dictionary<string, object> v)
            {
                return JsonConvert.DeserializeObject<Block>(JsonConvert.SerializeObject(v));
            }
            public static implicit operator Dictionary<string, object>(Block v)
            {
                Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(v));
                // Have to manually convert the items array, becuase that wants to cast to a JArray
                dict["items"] = v.items.Select<Item, Dictionary<string, object>>(i => i).ToArray();
                return dict;
            }
        }
        public Block[] blocks;

        public static implicit operator ItemSet(Dictionary<string, object> v)
        {
            return JsonConvert.DeserializeObject<ItemSet>(JsonConvert.SerializeObject(v));
        }
        public static implicit operator Dictionary<string, object>(ItemSet v)
        {
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(v));
            // Have to manually convert the blocks array, becuase that wants to cast to a JArray
            dict["blocks"] = v.blocks.Select<Block, Dictionary<string, object>>(i => i).ToArray();
            return dict;
        }
    }
}
