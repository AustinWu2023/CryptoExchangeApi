namespace CryptoExchangeApi.Share {
    public static class DateTimeHelper {
       
        public static string? ConvertUtcToLocalTimeStr(DateTime? utcTime) {
            return utcTime.HasValue
                ? utcTime.Value.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss")
                : string.Empty;
        }
    }
}
