using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rblt.Tools
{
    public static class Convert
    {
        #region Converters

        /// <summary>
        /// Encodes and shortens a series of timestamps into BASE64 string.
        /// </summary>
        /// <param name="timeStamps">A series of timestamp instances of type ITimeStampUTC.</param>
        /// <returns>The corresponding BASE64 encoded string.</returns>
        /// <remarks>
        /// The timestamps are encoded in the following way, and the encoded data consists of 32 bits blocks:
        /// 
        ///     - first block contains the first timestamp (passed seconds since 2000-01-01.)
        ///     - any subsequent block contains the difference in seconds from its previous timestamp
        ///         -- a block can hold up to 4 subsequent differences (slots), according to the greatness of
        ///         the values:
        ///         
        ///                  (*) flag      maxvalue     count
        ///                         0        2^30-1         1
        ///                         1        2^15-1         2
        ///                         2        2^10-1         3
        ///                         3         2^7-1         4
        ///                         
        ///          (*) When a block is encoded, the first two bits will hold the value of the block's flag,
        ///          aka. the number of differences minus one.
        ///          -- at the last block, if more slots could be used, than the number of differences left,
        ///          the flag is set according to the remaining count.
        /// </remarks>
        public static string EncodeTimeStamps(IEnumerable<DateTime> timeStamps)
        {
            var items = timeStamps.ToArray();
            if (items.Length == 0) return null;

            List<uint> diffs = new List<uint>();
            int i = 1;
            uint[] buckets = new uint[4];
            while (i < items.Length)
            {
                int bucketPtr = 0,
                    flag = 4;

                uint diff = (uint)(items[i] - items[i - 1]).TotalSeconds;
                do
                {
                    if (diff < (1 << (30 / flag)))
                    {
                        buckets[bucketPtr] = diff;
                        bucketPtr++;
                        i++;

                        if (i < items.Length)
                        {
                            if (items[i - 1] > items[i])
                                throw new ArgumentOutOfRangeException("Negative difference. Timestamp sequence is not monotonic.", (Exception)null);
                            else
                                diff = (uint)(items[i] - items[i - 1]).TotalSeconds;
                        }
                    }
                    else
                    {
                        flag--;
                    }
                }
                while (flag > 0 && bucketPtr < flag && i < items.Length);

                if (flag == 0)
                    throw new OverflowException();

                uint block = 0;
                for (int j = 0; j < bucketPtr; j++)
                    block |= buckets[bucketPtr - 1 - j] << ((30 / bucketPtr) * j);

                block |= (uint)(bucketPtr - 1) << 30;

                diffs.Add(block);
            }

            uint startDay = (uint)(items[0] - new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            byte[] buffer = BitConverter.GetBytes(startDay);

            buffer = buffer.Concat(diffs.SelectMany(b => BitConverter.GetBytes(b))).ToArray();

            return System.Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// Decodes the string representation of a series of timestamps.
        /// </summary>
        /// <param name="encodedTimeStamps">The encoded timestamps.</param>
        /// <returns>The corresponding array of DateTime instances.</returns>
        public static DateTime[] DecodeTimeStamps(string encodedTimeStamps)
        {
            if (String.IsNullOrEmpty(encodedTimeStamps)) return new DateTime[0];

            using (var stream = new MemoryStream(System.Convert.FromBase64String(encodedTimeStamps), false))
            {
                if (stream.Length < 2) throw new FormatException("Invalid byte length.");
                stream.Position = 0;

                using (BinaryReader reader = new BinaryReader(stream))
                {
                    var head = reader.ReadUInt32();
                    var beginDate = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(head);

                    List<DateTime> timeStamps = new List<DateTime>();

                    Func<uint, uint[]> getDiffs = (uint bytes) =>
                    {
                        uint flag = bytes >> 30,
                            blocks = flag + 1,
                            blocksize = 30 / blocks,
                            mask = (uint)(1 << ((int)blocksize)) - 1;

                        var result = new uint[blocks];
                        for (int i = 0; i < blocks; i++)
                            result[(blocks - 1) - i] = (bytes >> (int)(i * blocksize)) & mask;

                        return result;
                    };

                    var prevDate = beginDate;
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        timeStamps.AddRange(getDiffs(reader.ReadUInt32()).Select(diff => (prevDate = prevDate.AddSeconds(diff))));
                    }

                    timeStamps.Insert(0, beginDate);
                    return timeStamps.ToArray();
                }
            }
        }

        #endregion
    }
}
