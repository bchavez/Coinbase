using FluentAssertions;
using NUnit.Framework;

namespace Coinbase.Tests
{
   public class WebhookTests
   {
      [Test]
      public void can_validate()
      {
         var body = @"{""order"":{""id"":null,""created_at"":null,""status"":""completed"",""event"":null,""total_btc"":{""cents"":100000000,""currency_iso"":""BTC""},""total_native"":{""cents"":1000,""currency_iso"":""USD""},""total_payout"":{""cents"":1000,""currency_iso"":""USD""},""custom"":""123456789"",""receive_address"":""mzVoQenSY6RTBgBUcpSBTBAvUMNgGWxgJn"",""button"":{""type"":""buy_now"",""name"":""Test Item"",""description"":null,""id"":null},""transaction"":{""id"":""53bdfe4d091c0d74a7000003"",""hash"":""4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b"",""confirmations"":0}}}";
         var signature = @"6yQRl17CNj5YSHSpF+tLjb0vVsNVEv021Tyy1bTVEQ69SWlmhwmJYuMc7jiDyeW9TLy4vRqSh4g4YEyN8eoQIM57pMoNw6Lw6Oudubqwp+E3cKtLFxW0l18db3Z/vhxn5BScAutHWwT/XrmkCNaHyCsvOOGMekwrNO7mxX9QIx21FBaEejJeviSYrF8bG6MbmFEs2VGKSybf9YrElR8BxxNe/uNfCXN3P5tO8MgR5wlL3Kr4yq8e6i4WWJgD08IVTnrSnoZR6v8JkPA+fn7I0M6cy0Xzw3BRMJAvdQB97wkobu97gFqJFKsOH2u/JR1S/UNP26vL0mzuAVuKAUwlRn0SUhWEAgcM3X0UCtWLYfCIb5QqrSHwlp7lwOkVnFt329Mrpjy+jAfYYSRqzIsw4ZsRRVauy/v3CvmjPI9sUKiJ5l1FSgkpK2lkjhFgKB3WaYZWy9ZfIAI9bDyG8vSTT7IDurlUhyTweDqVNlYUsO6jaUa4KmSpg1o9eIeHxm0XBQ2c0Lv/T39KNc/VOAi1LBfPiQYMXD1e/8VuPPBTDGgzOMD3i334ppSr36+8YtApAn3D36Hr9jqAfFrugM7uPecjCGuleWsHFyNnJErT0/amIt24Nh1GoiESEq42o7Co4wZieKZ+/yeAlIUErJzK41ACVGmTnGoDUwEBXxADOdA=";

         WebhookHelper.IsValid(body, signature)
            .Should().BeTrue();
      }
   }
}