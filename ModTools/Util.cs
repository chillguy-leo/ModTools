using System.Text.RegularExpressions;

namespace SCPReplacer
{
    public static class Util
    {
        /// <summary>
        /// Given a stirng, capitalize the first character and make the rest lowercase.
        /// </summary>
        /// <param name="input">A string in any case</param>
        /// <returns>The string with only the first character capitalized</returns>
        public static string SentenceCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
    }
}