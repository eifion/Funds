namespace LibFunds.Extensions;

public static class StringExtensions
{
    public static bool TryParseDate(this string dateValue, out DateOnly date)
    {
        if (string.IsNullOrWhiteSpace(dateValue) || string.Equals(dateValue, "today", StringComparison.InvariantCultureIgnoreCase))
        {
            date = DateOnly.FromDateTime(DateTime.Today);
            return true;
        }

        if (string.Equals(dateValue, "yesterday", StringComparison.InvariantCultureIgnoreCase))
        {
            date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
            return true;
        }

        return DateOnly.TryParseExact(dateValue, "yyyy-MM-dd", out date);
    }
}