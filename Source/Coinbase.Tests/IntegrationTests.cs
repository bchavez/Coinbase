using System;
using System.Collections.Generic;
using System.Net;
using Coinbase.ObjectModel;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace Coinbase.Tests
{
   [TestFixture]
   public class IntegrationTests
   {
      private const string ApiKey = "l09yJNB38UjpU3eh";
      private const string ApiSecretKey = "0l1EfqtS5mRSt3tnpDJZW2umOdjIJaE7";
      private WebProxy proxy = new WebProxy("http://localhost.:8888", false);

      [Test]
      [Explicit]
      public void create_customer_checkout()
      {
         var api = new CoinbaseApi(ApiKey, ApiSecretKey, useSandbox: true, proxy: proxy, useTimeApi: true);

         var purchaseId = Guid.NewGuid().ToString("n");

         var request = new CheckoutRequest
            {
               Amount = 10.00m,
               Currency = "USD",
               Name = "Test Order",
               Metadata =
                  {
                     {"purchaseId", purchaseId}
                  },
               NotificationsUrl = ""
            };

         request.Dump();

         var checkout = api.CreateCheckout(request);

         checkout.Dump();
         if( checkout.Errors?.Length == 0 )
         {
            var redirectUrl = api.GetCheckoutUrl(checkout);
            //do redirect with redirectUrl
         }
         else
         {
            //Error making checkout page.
         }

         var checkoutUrl = api.GetCheckoutUrl(checkout);
         checkoutUrl.Should().StartWith("https://sandbox.coinbase.com/checkouts");
      }

      [Test]
      [Explicit]
      public void refund_example()
      {
         var api = new CoinbaseApi(ApiKey, ApiSecretKey, useSandbox: true, proxy: proxy, useTimeApi: true);

         var options = new
            {
               currency = "BTC"
            };

         var orderId = "ORDER_ID_HERE";

         var response = api.SendRequest($"/orders/{orderId}/refund", options);
         //process response as needed
      }


      [Test]
      public void test_callback_verification()
      {
         var json =
            @"{""order"":{""id"":null,""uuid"":""54b2d0f8-2bb8-5771-b330-9366ac079d60"",""resource_path"":""/v2/orders/54b2d0f8-2bb8-5771-b330-9366ac079d60"",""metadata"":null,""created_at"":null,""status"":""completed"",""event"":null,""total_btc"":{""cents"":100000000.0,""currency_iso"":""BTC""},""total_native"":{""cents"":1000000.0,""currency_iso"":""USD""},""total_payout"":{""cents"":1000000.0,""currency_iso"":""USD""},""custom"":""123456789"",""receive_address"":""mpAWHN5o3PtqQbu2WFsGgwx6Zep9pHoMyp"",""button"":{""type"":""buy_now"",""subscription"":false,""repeat"":null,""name"":""Test Item"",""description"":null,""id"":null,""uuid"":""ad92ae20-be4b-523d-aac6-6ff14ac2cc21"",""resource_path"":""/v2/checkouts/ad92ae20-be4b-523d-aac6-6ff14ac2cc21""},""transaction"":{""id"":""566656b6804ce90001003b2a"",""hash"":""4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b"",""confirmations"":0}}}";

         //X-Signature: qj7s3QLVlzkqrJdvH556ZvoM2dgeEwpXzkF0YPN4AR9GKL/F9kLcBmUarapMBAE5ef/QDcxN3ta8dBaTC1KYz+04+SriqvnuZzC3EXX5ksvxpN+i+6JjPIKsU63l1bk+t7GQvKeEGrq3/Al1OklVj9IMUnK5Sz59eUIByWgWBtVS0U+zI7ijwOoSZxkcAdY7l+EWa4t/W8gOChtA/2vdSgqtAVW/GcUMZ1ZFsH1U/EJIsY5Xw4dAbwtavIvABBBlmUTWUvj5Wubp7MaIptFfAL6fU1YwmbnsaKn6L+c4fTNsbOLCLYY7Czb4Um0MQ+A2/w1Ldwv2L9ie1b956y3lnbU/8qn6q0AU3FYOgepwmvoNRaca/ZXVniVZsY3PSX/zHf3WCjDh+jvAO6vneVpBlNNIyJjgGNzGdQzxzOhXJu5cFXJa1zXcPgELHtFSE2raG0tIdgwlgJwiYCls77R+3iMMv8CPat3a/GQ61Cax0Gsi80NaiavCgWjpMbXYRZvNP3DdUNVOvucIrtSWydq6bccQTamLCMpQ8fjB2zGcDglIl0I+W/6gtY1MflZ+ZmUySgEEww4PXlySYivvFwc/jpcptQ2vLXNjMZQmf9kbiRBe8PT/tpPUchUxNYzADDpSWeqBid0Jm7Um8G8zLwtBo5R1hHME9Gd/v3DOUk5sMzk=
         var signature =
            "qj7s3QLVlzkqrJdvH556ZvoM2dgeEwpXzkF0YPN4AR9GKL/F9kLcBmUarapMBAE5ef/QDcxN3ta8dBaTC1KYz+04+SriqvnuZzC3EXX5ksvxpN+i+6JjPIKsU63l1bk+t7GQvKeEGrq3/Al1OklVj9IMUnK5Sz59eUIByWgWBtVS0U+zI7ijwOoSZxkcAdY7l+EWa4t/W8gOChtA/2vdSgqtAVW/GcUMZ1ZFsH1U/EJIsY5Xw4dAbwtavIvABBBlmUTWUvj5Wubp7MaIptFfAL6fU1YwmbnsaKn6L+c4fTNsbOLCLYY7Czb4Um0MQ+A2/w1Ldwv2L9ie1b956y3lnbU/8qn6q0AU3FYOgepwmvoNRaca/ZXVniVZsY3PSX/zHf3WCjDh+jvAO6vneVpBlNNIyJjgGNzGdQzxzOhXJu5cFXJa1zXcPgELHtFSE2raG0tIdgwlgJwiYCls77R+3iMMv8CPat3a/GQ61Cax0Gsi80NaiavCgWjpMbXYRZvNP3DdUNVOvucIrtSWydq6bccQTamLCMpQ8fjB2zGcDglIl0I+W/6gtY1MflZ+ZmUySgEEww4PXlySYivvFwc/jpcptQ2vLXNjMZQmf9kbiRBe8PT/tpPUchUxNYzADDpSWeqBid0Jm7Um8G8zLwtBo5R1hHME9Gd/v3DOUk5sMzk=";

         var api = new CoinbaseApi(ApiKey, ApiSecretKey, useSandbox: true, proxy: proxy, useTimeApi: true);

         var note = JsonConvert.DeserializeObject<Notification>(json, api.JsonSettings);

         //            note.IsVerified.Should().BeFalse();
         note.IsVerified.Should().BeFalse();

         //api.VerifyNotification(json, signature)
         //    .Should().BeTrue();
      }

      [Test]
      [Explicit]
      public void can_list_orders()
      {
         var api = new CoinbaseApi(ApiKey, ApiSecretKey, useSandbox: true, proxy: proxy, useTimeApi: true);

         var response = api.SendRequest("orders", null, Method.GET);

         response.Dump();
      }

      [Test]
      public void can_deser_list_response()
      {
         var json =
            @"{""pagination"":{""ending_before"":null,""starting_after"":null,""limit"":25,""order"":""desc"",""previous_uri"":null,""next_uri"":null},""data"":[{""id"":""cca4ac42-7db1-51b9-bad6-73553ca341b9"",""code"":""W020I6OB"",""type"":""order"",""name"":""Test Order"",""description"":null,""amount"":{""amount"":""10.00"",""currency"":""USD""},""receipt_url"":""https://www.coinbase.com/orders/2a9419b5e6258059bc52dfb9cfd0b08f/receipt"",""resource"":""order"",""resource_path"":""/v2/orders/cca4ac42-7db1-51b9-bad6-73553ca341b9"",""status"":""expired"",""bitcoin_amount"":{""amount"":""0.00100000"",""currency"":""BTC""},""payout_amount"":null,""bitcoin_address"":""n2hEf3xgQ9gLSUrYy8ZCP2Zw8TFyboqRk3"",""refund_address"":null,""bitcoin_uri"":""bitcoin:n2hEf3xgQ9gLSUrYy8ZCP2Zw8TFyboqRk3?amount=0.001\u0026r=https://sandbox.coinbase.com/r/56664c7b27aee501d2000057"",""notifications_url"":null,""paid_at"":null,""mispaid_at"":null,""expires_at"":""2015-12-08T03:35:27Z"",""metadata"":{},""created_at"":""2015-12-08T03:20:27Z"",""updated_at"":""2015-12-08T03:20:27Z"",""customer_info"":null,""transaction"":null,""mispayments"":[],""refunds"":[]},{""id"":""7e1d9170-cc8b-5468-ba7f-a852759854f8"",""code"":""B4K9P5ZL"",""type"":""order"",""name"":""Test Order"",""description"":null,""amount"":{""amount"":""10.00"",""currency"":""USD""},""receipt_url"":""https://www.coinbase.com/orders/bf6b871ba37ed6332e77a19d3c556544/receipt"",""resource"":""order"",""resource_path"":""/v2/orders/7e1d9170-cc8b-5468-ba7f-a852759854f8"",""status"":""expired"",""bitcoin_amount"":{""amount"":""0.00100000"",""currency"":""BTC""},""payout_amount"":null,""bitcoin_address"":""n1Cn2gwP674dau9D459bh29xsYY3EkPP1g"",""refund_address"":null,""bitcoin_uri"":""bitcoin:n1Cn2gwP674dau9D459bh29xsYY3EkPP1g?amount=0.001\u0026r=https://sandbox.coinbase.com/r/566649e04d3af201f800001f"",""notifications_url"":null,""paid_at"":null,""mispaid_at"":null,""expires_at"":""2015-12-08T03:24:20Z"",""metadata"":{},""created_at"":""2015-12-08T03:09:20Z"",""updated_at"":""2015-12-08T03:09:20Z"",""customer_info"":null,""transaction"":null,""mispayments"":[],""refunds"":[]}]}";

         var obj = JsonConvert.DeserializeObject<CoinbaseResponse>(json);

         obj.Dump();
      }
   }
}