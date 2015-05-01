using System;
using System.Text;

namespace Quilt4.BusinessEntities
{
    public class RandomUtility
    {
        private static readonly Random Rng = new Random((int)DateTime.UtcNow.Ticks);

        public class CharInterval
        {
            public int Interval;
            public char Chr;
        }

        public static string GetRandomString(int size, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", CharInterval charInterval = null)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < size; i++)
            {
                char ch;
                if (charInterval != null && (i + 1) % (charInterval.Interval + 1) == 0)
                    ch = charInterval.Chr;
                else
                {
                    //var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Rng.NextDouble() + 65)));
                    var index = Convert.ToInt32(Math.Floor(chars.Length * Rng.NextDouble()));
                    ch = chars[index];
                }
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}
