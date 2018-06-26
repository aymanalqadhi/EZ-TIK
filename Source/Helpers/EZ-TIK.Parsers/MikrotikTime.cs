using System.Text;
using System.Text.RegularExpressions;

namespace EZ_TIK.Parsers
{
    public class MikrotikTime
    {
        #region Public Properties

        /// <summary>
        /// The days in the mikrotik time string
        /// </summary>
        public int Days { get; private set; }

        /// <summary>
        /// The hours in the mikrotik time string
        /// </summary>
        public int Hours { get; private set; }

        /// <summary>
        /// The mintues in the mikrotik time string
        /// </summary>
        public int Mintues { get; private set; }

        /// <summary>
        /// The seconds in the mikrotik time string
        /// </summary>
        public int Seconds { get; private set; } 

        #endregion

        /// <summary>
        /// Static constrcutor
        /// </summary>
        /// <param name="time">the time string from the server</param>
        /// <returns></returns>
        public static MikrotikTime Parse(string time)
        {
            if (time == null || !Regex.IsMatch(time, @"(\d+:\d+:\d+)|(\d+[a-z])") || time.Trim().Equals("0s")) return null;

            var parsed = new MikrotikTime();

            if (Regex.IsMatch(time, @"(\d+)w"))
                parsed.Days += int.Parse(Regex.Match(time, @"(\d+)w").Groups[1].Value) * 7;

            if (Regex.IsMatch(time, @"(\d+)d"))
                parsed.Days += int.Parse(Regex.Match(time, @"(\d+)d").Groups[1].Value);

            if(Regex.IsMatch(time, @"\d+:\d+:\d+"))
            {
                parsed.Hours = int.Parse(Regex.Match(time, @"(\d+):\d+:\d+").Groups[1].Value);
                parsed.Mintues = int.Parse(Regex.Match(time, @"\d+:(\d+):\d+").Groups[1].Value);
                parsed.Seconds = int.Parse(Regex.Match(time, @"\d+:\d+:(\d+)").Groups[1].Value);
            }

            if (Regex.IsMatch(time, @"(\d+)h"))
                parsed.Hours += int.Parse(Regex.Match(time, @"(\d+)h").Groups[1].Value);

            if (Regex.IsMatch(time, @"(\d+)m"))
                parsed.Mintues += int.Parse(Regex.Match(time, @"(\d+)m").Groups[1].Value);

            return parsed;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Days > 0) sb.Append($"{Days} Days");
            if (Hours > 0) sb.Append($"{(sb.Length > 0 ? ", " : string.Empty)}{Hours} Hours");
            if (Mintues > 0) sb.Append($"{(sb.Length > 0 ? ", " : string.Empty)}{Mintues} Minutes");

            return sb.ToString();
        }
    }
}
