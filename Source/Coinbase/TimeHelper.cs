using System;

namespace Coinbase
{
   public static class TimeHelper
   {
      private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

      public static long GetCurrentUnixTimestampSeconds()
      {
#if STANDARD
         return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
#else
         return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
#endif
      }

      public static DateTimeOffset FromUnixTimestampSeconds(long seconds)
      {
#if STANDARD
         return DateTimeOffset.FromUnixTimeSeconds(seconds);
#else
         return UnixEpoch.AddSeconds(seconds);
#endif
      }
   }
}