using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

public class Utils
{
    // Helper method
    public static DateTime ConvertDate(string date)
    {
        return DateTime.ParseExact(date, "dd-MM-yyyy",
                                                      CultureInfo.InvariantCulture);
    }

    public static bool TryParseDate(string strDate, out DateTime date)
    {
        try
        {
            date = DateTime.ParseExact(strDate, "dd-MM-yyyy",
                                                      CultureInfo.InvariantCulture);
            return true;
        }
        catch /*(Exception ex)*/
        {
            date = DateTime.Now;

            return false;
        }
    }
}
