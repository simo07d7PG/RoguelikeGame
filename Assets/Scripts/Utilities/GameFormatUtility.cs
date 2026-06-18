using System.Text;
using UnityEngine;

namespace FinalRogue
{
    public static class GameFormatUtility
    {
        public static string FormatTime(float seconds)
        {
            int total = Mathf.FloorToInt(seconds);
            return $"{total / 60:00}:{total % 60:00}";
        }

        public static string BuildLeaderboardText(SaveManager saveManager)
        {
            if (saveManager == null)
                return "Leaderboard";

            var builder = new StringBuilder("Leaderboard\n");
            int rank = 1;
            foreach (var entry in saveManager.Leaderboard)
            {
                builder.AppendLine($"{rank}. {entry.nickname} - {FormatTime(entry.playTimeSeconds)}");
                rank++;
            }

            return builder.ToString();
        }
    }
}