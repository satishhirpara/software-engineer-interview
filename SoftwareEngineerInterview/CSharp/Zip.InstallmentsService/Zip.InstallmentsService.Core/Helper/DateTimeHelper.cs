using System;
using System.Collections.Generic;
using System.Text;

namespace Zip.InstallmentsService.Core.Helper
{
    /// <summary>
    /// Common methods related to datime operations
    /// </summary>
    public static class DateTimeHelper
    {
        public static DateTime GetNextDateAfterDays(DateTime date, int days)
        {
            return date.AddDays(days).Date;
        }

    }
}
