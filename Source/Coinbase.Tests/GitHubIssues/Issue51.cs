using System;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Coinbase.Tests.GitHubIssues
{
   public class Issue51 : OAuthServerTest
   {
      private const string SellOrder = @"{
    ""id"": ""74a3b85b-1cde-473d-a922-5102a3306e46"",
    ""fee"": { ""amount"": ""0.99"", ""currency"": ""EUR"" },
    ""status"": ""created"",
    ""user_reference"": ""FFFF"",
    ""transaction"": null,
    ""created_at"": null,
    ""updated_at"": null,
    ""resource"": ""sell"",
    ""resource_path"": ""/v2/accounts/74a3b85b-1cde-473d-a922-5102a3306e46/sells/74a3b85b-1cde-473d-a922-5102a3306e46"",
    ""payment_method"": {
      ""id"": ""74a3b85b-1cde-473d-a922-5102a3306e46"",
      ""resource"": ""payment_method"",
      ""resource_path"": ""/v2/payment-methods/74a3b85b-1cde-473d-a922-5102a3306e46""
    },
    ""amount"": { ""amount"": ""0.04000000"", ""currency"": ""LTC"" },
    ""total"": { ""amount"": ""0.12"", ""currency"": ""EUR"" },
    ""subtotal"": { ""amount"": ""1.11"", ""currency"": ""EUR"" },
    ""unit_price"": { ""amount"": ""27.75"", ""currency"": ""EUR"", ""scale"": 2 },
    ""payout_at"": ""2019-01-17T17:16:34Z"",
    ""committed"": false,
    ""instant"": true
  }";

      [Test]
      public async Task can_deser_sellorder()
      {
         SetupServerSingleResponse(SellOrder);


         var create = new PlaceSell
            {
               Amount = 10m,
               Currency = "BTC",
               PaymentMethod = "B28EB04F-BD70-4308-90A1-96065283A001"
            };

         Func<Task<Response<Sell>>> action = async () => await client.Sells.PlaceSellOrderAsync("fff", create);

         action.Should().NotThrow();
      }

   }
}
