using System;
using System.Collections.Generic;

namespace Game
{
    public class LeaderboardEntry
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
}