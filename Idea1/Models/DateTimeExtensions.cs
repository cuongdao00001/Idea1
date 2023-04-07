using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea1.Models
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTime ToDateTime(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.DateTime;
        }
    }
}