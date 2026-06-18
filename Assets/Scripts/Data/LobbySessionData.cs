using UnityEngine;

namespace FinalRogue
{
    public static class LobbySessionData
    {
        const string PrefNickname = "finalrogue_player_nickname";

        public static string LoadNickname() =>
            PlayerPrefs.GetString(PrefNickname, NicknameUtility.Default);

        public static void SaveNickname(string nickname)
        {
            PlayerPrefs.SetString(PrefNickname, NicknameUtility.Normalize(nickname));
            PlayerPrefs.Save();
        }
    }
}