using System;

namespace Zip.InstallmentsService.Core.Extension
{
    /// <summary>
    /// Extension methods related to datime operations
    /// </summary>
    public static class DateTimeExtension
    {
        public static DateTime GetNextDateAfterDays(this DateTime date, int days)
        {
            return date.AddDays(days).Date;
        }

    }
}
