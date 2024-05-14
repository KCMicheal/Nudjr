namespace Nudjr_AppCore.Services.Extensions
{
    public static class Extensions
    {
        public static double ConvertToTimeStamp(this DateTime dateInstance)
        {
            DateTime epochDateTime = new DateTime(1970, 1, 1);
            return (dateInstance - epochDateTime).TotalMilliseconds;
        }

        public static DateTime ConvertToDate(this long timestamp)
        {
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
            return dateTime;
        }
    }
}
