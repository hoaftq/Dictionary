namespace CollectData
{
    static class StringExtension
    {
        public static string TrimAllSpecialCharacters(this string str)
        {
            return str?.Replace("&nbsp;", " ").Replace("&amp;", "&").Trim().Trim('\n', '\r', ':');
        }
    }
}
