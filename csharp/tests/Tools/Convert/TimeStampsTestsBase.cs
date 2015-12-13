using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rblt.Tests
{
    public class TimeStampsTestBase
    {
        public static void SetupMocks()
        {
            CurrentDate = new DateTime(2015, 01, 01, 12, 0, 0, DateTimeKind.Utc);

            DateTime prevDate = CurrentDate;
            TimeStamps1 = new DateTime[] { 
                // heading
                CurrentDate,
                // 4 slots (7 bits) block
                (prevDate = prevDate.AddSeconds(1)),
                (prevDate = prevDate.AddSeconds(32)),
                (prevDate = prevDate.AddSeconds(64)),
                (prevDate = prevDate.AddSeconds(127)),
                // 3 slots (10 bits) block
                (prevDate = prevDate.AddSeconds(255)),
                (prevDate = prevDate.AddSeconds(511)),
                (prevDate = prevDate.AddSeconds(1023)),
                // 2 slots (15 bits) block
                (prevDate = prevDate.AddSeconds(8191)),
                (prevDate = prevDate.AddSeconds(65535)),
                // 1 slot (30 bits) block
                (prevDate = prevDate.AddSeconds(1073741823)),
            };

            prevDate = CurrentDate;
            TimeStamps2 = new DateTime[] { 
                CurrentDate,
                // ^^ heading
                (prevDate = prevDate.AddSeconds(1)),
                (prevDate = prevDate.AddSeconds(32)),
                (prevDate = prevDate.AddSeconds(64)),
                (prevDate = prevDate.AddSeconds(127)),
                // ^^ should be the 1st block ( 4 x 7 bits )
                (prevDate = prevDate.AddSeconds(127)),
                (prevDate = prevDate.AddSeconds(127)),
                (prevDate = prevDate.AddSeconds(1023)),
                // ^^ should be the 2nd block ( 3 x 10 bits )
                (prevDate = prevDate.AddSeconds(1023)),
                (prevDate = prevDate.AddSeconds(65535)),
                // ^^ should be the 3rd block ( 2 x 15 bits )
                (prevDate = prevDate.AddSeconds(65535)),
                // ^^ should be the 4th block ( 1 x 30 bits )
                (prevDate = prevDate.AddSeconds(1073741823)),
                // ^^ should be the 5th block ( 1 x 30 bits )
                (prevDate = prevDate.AddSeconds(1))
                // ^^ should be the last block ( 1 x 30 bits )
            };

            var random = new Random();
            prevDate = CurrentDate;
            RandomTimeStamps = Enumerable.Range(0, random.Next(20))
                .Select(i => (prevDate = prevDate.AddSeconds(random.Next(100))))
                .ToArray();
        }

        public static DateTime CurrentDate;
        public static DateTime[] TimeStamps1;
        public static DateTime[] TimeStamps2;
        public static DateTime[] RandomTimeStamps;
    }
}
