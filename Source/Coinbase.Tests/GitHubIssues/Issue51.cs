using System;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using NUnit.Framework;
using static Coinbase.Tests.Examples;

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

         Func<Task<Response<Sell>>> action = async () =>
            await client.Sells.PlaceSellOrderAsync("fff", create);

         action.Should().NotThrow();
      }


      private const string PaymentMethods = @"{
      ""id"": ""ee12b3d9-7476-4f88-a7c4-b11ed8b8fa0a"",
      ""type"": ""secure3d_card"",
      ""name"": ""///"",
      ""currency"": ""EUR"",
      ""primary_buy"": false,
      ""primary_sell"": false,
      ""allow_buy"": false,
      ""allow_sell"": false,
      ""allow_deposit"": false,
      ""allow_withdraw"": false,
      ""instant_buy"": true,
      ""instant_sell"": false,
      ""created_at"": ""2017-12-10T08:13:02Z"",
      ""updated_at"": ""2017-12-10T08:13:02Z"",
      ""resource"": ""payment_method"",
      ""resource_path"": ""/v2/payment-methods/ee12b3d9-7476-4f88-a7c4-b11ed8b8fa0a"",
      ""limits"": {
        ""type"": ""card"",
        ""name"": ""Credit/Debit Card"",
        ""buy"": [
          {
            ""period_in_days"": 7,
            ""total"": { ""amount"": ""1000.00"", ""currency"": ""EUR"" },
            ""remaining"": { ""amount"": ""1000.00"", ""currency"": ""EUR"" },
            ""description"": ""€1,000 of your €1,000 weekly card limit remaining"",
            ""label"": ""Weekly card limit"",
            ""next_requirement"": {
              ""type"": ""buy_history"",
              ""volume"": { ""amount"": ""1000.00"", ""currency"": ""USD"" },
              ""amount_remaining"": { ""amount"": ""00"", ""currency"": ""USD"" },
              ""time_after_starting"": 2592000
            }
          }
        ],
        ""deposit"": [
          {
            ""period_in_days"": 7,
            ""total"": { ""amount"": ""1000.00"", ""currency"": ""EUR"" },
            ""remaining"": { ""amount"": ""1000.00"", ""currency"": ""EUR"" },
            ""description"": ""€1,000 of your €1,000 weekly card limit remaining"",
            ""label"": ""Weekly card limit""
          }
        ]
      },
      ""verified"": true
    },
    {
      ""id"": ""ee12b3d9-7476-4f88-a7c4-b11ed8b8fa0a"",
      ""type"": ""fiat_account"",
      ""name"": ""EUR Wallet"",
      ""currency"": ""EUR"",
      ""primary_buy"": true,
      ""primary_sell"": true,
      ""allow_buy"": true,
      ""allow_sell"": true,
      ""allow_deposit"": true,
      ""allow_withdraw"": true,
      ""instant_buy"": true,
      ""instant_sell"": true,
      ""created_at"": ""2017-12-10T08:07:47Z"",
      ""updated_at"": ""2017-12-10T08:07:47Z"",
      ""resource"": ""payment_method"",
      ""resource_path"": ""/v2/payment-methods/ee12b3d9-7476-4f88-a7c4-b11ed8b8fa0a"",
      ""limits"": { ""type"": ""fiat_account"", ""name"": ""Coinbase Account"" },
      ""fiat_account"": {
        ""id"": ""ee12b3d9-7476-4f88-a7c4-b11ed8b8fa0a"",
        ""resource"": ""account"",
        ""resource_path"": ""/v2/accounts/ee12b3d9-7476-4f88-a7c4-b11ed8b8fa0a""
      },
      ""verified"": true
    }";

      [Test]
      public async Task can_list_payment_methods()
      {
         SetupServerPagedResponse(PaginationJson, PaymentMethods);
         
         Func<Task<PagedResponse<PaymentMethod>>> action = async () =>
            await client.PaymentMethods.ListPaymentMethodsAsync();

         action.Should().NotThrow();
      }

   }
}
