using System;
using System.Linq;
using FluentAssertions;
using Flurl.Http.Testing;
using Newtonsoft.Json;
using Z.ExtensionMethods;

namespace Coinbase.Tests
{
   internal static class ExtensionsForTesting
   {
      public static void Dump(this object obj)
      {
         Console.WriteLine(obj.DumpString());
      }

      public static string DumpString(this object obj)
      {
         return JsonConvert.SerializeObject(obj, Formatting.Indented);
      }

      public static HttpCallAssertion ShouldHaveRequestBody(this HttpTest test, string json)
      {
         test.CallLog.First().RequestBody.Should().Be(json);
         return new HttpCallAssertion(test.CallLog);
      }

      public static bool IsAppVeyor(this OperatingSystem os)
      {
         return Environment.GetEnvironmentVariable("APPVEYOR").IsNotNullOrWhiteSpace();
      }
   }
}
