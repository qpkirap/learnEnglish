using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Game
{
    public partial class LeaderboardEntry
    {
        public readonly string firebaseId;
        public readonly int pointClick;

        public LeaderboardEntry(string firebaseId, int pointClick)
        {
            this.firebaseId = firebaseId;
            this.pointClick = pointClick;
        }
        
        public Dictionary<string, Object> ToDictionary() {
            Dictionary<string, Object> result = new Dictionary<string, Object>();
            result[SaveKeys.firebaseIdKey] = firebaseId;
            result[SaveKeys.pointClickKey] = pointClick;

            return result;
        }
    }
    
    public partial class LeaderboardEntry
    {
        public static LeaderboardEntry FromJson(string json) =>
            JsonConvert.DeserializeObject<LeaderboardEntry>(json, Settings);
        
        public static readonly JsonSerializerSettings Settings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Formatting = Formatting.Indented,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
        
        public string ToJson() => JsonConvert.SerializeObject(this, Settings);
    }
}