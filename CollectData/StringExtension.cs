namespace DataAccess.Utils
{
    static class StringExtension
    {
        public static string TrimNewLine(this string str)
        {
            return str?.Trim().Trim('\n', '\r');
        }
    }
}
