namespace MyWebApp.Data
{
    public class DateTimeWrapper
    {
        private static readonly string[] _monthAbbreviations = new string[] {
            "Jan", "Feb", "Mar",
            "Apr", "May", "Jun",
            "Jul", "Aug", "Sep",
            "Oct", "Nov", "Dec"
        };

        private string Format(int number)
        {
            return number.ToString().Length == 1 ? "0" + number : string.Empty + number;
        }

        public string GetMyTimeFormat(DateTime time)
        {
            var format = "{0}-{1}-{2} ({3}), {4} {5} {6}";

            return string.Format(format,
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
