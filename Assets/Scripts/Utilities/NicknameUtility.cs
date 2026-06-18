namespace FinalRogue
{
    public static class NicknameUtility
    {
        public const string Default = "Player";
        const int MaxLength = 10;

        public static string Normalize(string nickname)
        {
            if (string.IsNullOrWhiteSpace(nickname))
                return Default;

            nickname = nickname.Trim();
            return nickname.Length > MaxLength ? nickname.Substring(0, MaxLength) : nickname;
        }
    }
}