using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FinalRogue
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        const string FileName = "leaderboard.json";
        SaveData saveData = new();

        public IReadOnlyList<LeaderboardEntry> Leaderboard => saveData.leaderboard;

        public static void EnsureExists()
        {
            if (Instance != null)
                return;

            var saveObject = new GameObject("SaveManager");
            saveObject.AddComponent<SaveManager>();
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        string GetFilePath() => Path.Combine(Application.persistentDataPath, FileName);

        public void Load()
        {
            string path = GetFilePath();
            if (!File.Exists(path))
            {
                saveData = new SaveData();
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                saveData = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
            }
            catch
            {
                saveData = new SaveData();
            }

            if (saveData.leaderboard == null)
                saveData.leaderboard = new List<LeaderboardEntry>();

            TrimLeaderboard();
        }

        void TrimLeaderboard()
        {
            if (saveData.leaderboard.Count <= SaveData.MaxEntries)
                return;

            saveData.leaderboard.Sort((a, b) => b.playTimeSeconds.CompareTo(a.playTimeSeconds));
            saveData.leaderboard.RemoveRange(SaveData.MaxEntries, saveData.leaderboard.Count - SaveData.MaxEntries);
        }

        public void SaveLeaderboardEntry(string nickname, float playTimeSeconds)
        {
            saveData.leaderboard.Add(new LeaderboardEntry
            {
                nickname = NicknameUtility.Normalize(nickname),
                playTimeSeconds = playTimeSeconds
            });

            TrimLeaderboard();

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(GetFilePath(), json);
        }
    }
}