using System;
using System.Linq;

namespace EZ_TIK.Utils
{
    public static class StringUtils
    {
        private static readonly Random _rnd = new Random();

        /// <summary>
        /// A Method to generate a random string
        /// </summary>
        /// <param name="type">The type of the generated string</param>
        /// <param name="length">The length of the generated string</param>
        /// <param name="exChars">the characters to exclude form the string</param>
        /// <returns>The randomly generated string</returns>
        public static string GetRandomString(RandomStringKind type, int length, string exChars = "")
        {
            var chars = string.Empty;

            chars += type == RandomStringKind.CharactersOnly || type == RandomStringKind.Mixed ? "abcdefghijklmnopqrstuvwxyz" : "";
            chars += type == RandomStringKind.NumbersOnly || type == RandomStringKind.Mixed ? "1234567890" : "";

            exChars.ToList().ForEach(c => chars = chars.Replace(c.ToString(), ""));

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_rnd.Next(s.Length)]).ToArray());
        }

        
    }

    public enum RandomStringKind { CharactersOnly, NumbersOnly, Mixed }
}
