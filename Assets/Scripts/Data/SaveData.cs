using System;
using System.Collections.Generic;

namespace FinalRogue
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string nickname;
        public float playTimeSeconds;
    }

    [Serializable]
    public class SaveData
    {
        public const int MaxEntries = 5;
        public List<LeaderboardEntry> leaderboard = new();
    }
}