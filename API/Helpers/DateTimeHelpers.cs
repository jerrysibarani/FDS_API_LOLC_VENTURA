namespace API.Helpers
{
    public class DateTimeHelpers
    {

        public static DateTime? ToUtc(DateTime? date)
        {
            if (date == null) return null;
            return DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
        }

        public static DateTime? FixDate(DateTime? date)
        {
            if (date == null) return null;
            return DateTime.SpecifyKind(date.Value, DateTimeKind.Unspecified);
        }

    }
}