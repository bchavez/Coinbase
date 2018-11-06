using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Coinbase
{
   public static class ApiKeyAuthenticator
   {

      public static string GenerateSignature(string timestamp, string method, string url, string body, string appSecret)
      {
         return GetHMACInHex(appSecret, timestamp + method + url + body);
      }

      internal static string GetHMACInHex(string key, string data)
      {
         var hmacKey = Encoding.UTF8.GetBytes(key);
         var dataBytes = Encoding.UTF8.GetBytes(data);

         using( var hmac = new HMACSHA256(hmacKey) )
         {
            var sig = hmac.ComputeHash(dataBytes);
            return ByteToHexString(sig);
         }
      }

      //https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa/14333437#14333437
      static string ByteToHexString(byte[] bytes)
      {
         char[] c = new char[bytes.Length * 2];
         int b;
         for (int i = 0; i < bytes.Length; i++)
         {
            b = bytes[i] >> 4;
            c[i * 2] = (char)(87 + b + (((b - 10) >> 31) & -39));
            b = bytes[i] & 0xF;
            c[i * 2 + 1] = (char)(87 + b + (((b - 10) >> 31) & -39));
         }
         return new string(c);
      }

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