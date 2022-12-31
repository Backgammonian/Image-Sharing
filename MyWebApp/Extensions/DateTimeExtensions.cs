namespace MyWebApp.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly string[] _monthAbbreviations = new string[] { 
            "Jan",
            "Feb",
            "Mar",
            "Apr",
            "May",
            "Jun",
            "Jul",
            "Aug",
            "Sep",
            "Oct",
            "Nov",
            "Dec"
        };

        private static string Format(int number)
        {
            return number.ToString().Length == 1 ? "0" + number : "" + number;
        }

        public static string GetMyTimeFormat(this DateTime time)
        {
            return string.Format("{0}-{1}-{2} ({3}), {4} {5} {6}",
                time.Hour,
                Format(time.Minute),
                Format(time.Second),
                Format(time.Millisecond),
                time.Day,
                _monthAbbreviations[time.Month - 1],
                time.Year);
        }
    }
}
