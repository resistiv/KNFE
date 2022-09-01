using System;

namespace KNFE.Core.Utils
{
    public static class Time
    {
        /// <summary>
        /// Converts a 32-bit packed MS-DOS time to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dosDate">An unsigned short representing a packed DOS date.</param>
        /// <param name="dosTime">An unsigned short representing a packed DOS time.</param>
        /// <returns>A <see cref="DateTime"/> equivalent to the provided DOS date and time.</returns>
        public static DateTime ConvertDosDateTime(ushort dosDate, ushort dosTime)
        {
            /* --------------------------------------------------------------------------------
                    Year offset from 1980              Month                  Day
               ┌────────────────────────────┐     ┌─────────────┐     ┌──────────────────┐ 
            ┌────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┐
            │ 15 │ 14 │ 13 │ 12 │ 11 │ 10 │  9 │  8 │  7 │  6 │  5 │  4 │  3 │  2 │  1 │  0 │
            └────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┘
                   Hour (24 HR)                   Minute                  Seconds (/2)
               ┌──────────────────┐     ┌───────────────────────┐     ┌──────────────────┐ 
            ┌────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┐
            │ 15 │ 14 │ 13 │ 12 │ 11 │ 10 │  9 │  8 │  7 │  6 │  5 │  4 │  3 │  2 │  1 │  0 │
            └────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┘
            -------------------------------------------------------------------------------- */

            int year = ((dosDate >> 9) & 0x7F) + 1980;
            int month = (dosDate >> 5) & 0x0F;
            // Error correction, some formats utilize a "blank date"
            if (month == 0)
                month = 1;
            int day = dosDate & 0x1F;
            // See above
            if (day == 0)
                day = 1;
            int hour = (dosTime >> 11) & 0x1F;
            int minute = (dosTime >> 5) & 0x3F;
            int second = (dosTime & 0x1F) * 2;

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
