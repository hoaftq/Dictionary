namespace CollectData
{
    static class StringExtension
    {
        public static string TrimAllSpecialCharacters(this string str)
        {
            return str?.Trim().Trim('\n', '\r', ':');
        }
    }
}
