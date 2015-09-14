using System;
using System.Diagnostics;
using System.Linq;
using Coinbase.Mvc;
using Coinbase.ObjectModel;
using FluentAssertions;
using FluentAssertions.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace Coinbase.Tests
{
    [TestFixture]
    public class DeseralizationTests
    {
        [Test]
        public void button_json()
        {
            var json =
                @"{
  ""name"": ""test"",
  ""type"": ""buy_now"",
  ""price_string"": ""1.23"",
  ""price_currency_iso"": ""USD"",
  ""custom"": ""Order123"",
  ""callback_url"": ""http://www.example.com/my_custom_button_callback"",
  ""description"": ""Sample description"",
  ""style"": ""custom_large"",
  ""include_email"": true
}".RemoveNewLines();

            var b = JsonConvert.DeserializeObject<ButtonRequest>( json );

            b.Name.Should().Be( "test" );
            b.Type.Should().Be( ButtonType.BuyNow );
            b.Price.Should().Be( 1.23m );
            b.Currency.Should().Be( Currency.USD );
            b.Custom.Should().Be( "Order123" );
            b.CallbackUrl.Should().Be( "http://www.example.com/my_custom_button_callback" );
            b.Description.Should().Be( "Sample description" );
            b.Style.Should().Be( ButtonStyle.CustomLarge );
            b.IncludeEmail.Should().BeTrue();

            var settings = new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

            var output = JsonConvert.SerializeObject( b, Formatting.Indented, settings );


            Console.WriteLine( "INPUT>>" );
            Console.WriteLine( json );

            Console.WriteLine( "OUTPUT>>" );
            Console.WriteLine( output );


            output.RemoveNewLines().Should().Be( json );
        }

        [Test]
        public void should_be_able_to_parse_button_response()
        {
	        var json = @"{
   ""success"":true,
   ""button"":{
      ""code"":""39b2d09f2bf9c88f113aa6fecb13d258"",
      ""type"":""buy_now"",
      ""style"":""custom_large"",
      ""text"":""Pay With Bitcoin"",
      ""name"":""Order Name"",
      ""description"":""Order Description"",
      ""custom"":""Custom_Order_Id"",
      ""callback_url"":""http://www.bitarmory.com/callback"",
      ""success_url"":""http://www.bitarmory.com/success"",
      ""cancel_url"":""http://www.bitarmory.com/cancel"",
      ""info_url"":""http://www.bitarmory.com/info"",
      ""price"":{
         ""cents"":7999,
         ""currency_iso"":""USD""
      },
      ""variable_price"":false,
      ""choose_price"":false,
      ""include_address"":false,
      ""include_email"":false
   }
}";
            var obj = JsonConvert.DeserializeObject<ButtonResponse>( json );

            obj.Should().NotBeNull();

            var truth = new ButtonResponse()
                {
                    Success = true,
                    Button = new ButtonCreated()
                        {
                            Code = "39b2d09f2bf9c88f113aa6fecb13d258",
                            Type = ButtonType.BuyNow,
                            Style = ButtonStyle.CustomLarge,
                            Text = "Pay With Bitcoin",
                            Name = "Order Name",
                            Description = "Order Description",
                            Custom = "Custom_Order_Id",
                            CallbackUrl = "http://www.bitarmory.com/callback",
                            SuccessUrl = "http://www.bitarmory.com/success",
                            CancelUrl = "http://www.bitarmory.com/cancel",
                            InfoUrl = "http://www.bitarmory.com/info",
                            Price = new Price
                                {
                                    Cents = 7999,
                                    Currency = Currency.USD
                                },
                            VariablePrice = false,
                            ChoosePrice = false,
                            IncludeAddress = false,
                            IncludeEmail = false
                        }
                };

            obj.ShouldBeEquivalentTo( truth );
        }
        
        //Coinbase updated Button response due to API changes.
        [Test]
        public void should_be_able_to_parse_button_response_V2()
        {
            /*
             * {"success":false,
             *    "errors":["You have not filled out your merchant profile. Please enter your information in the Profile section."],
             *    "button": {
             *        "code":"c6fcf1709df34388c6a1eadc145db599",
             *        "type":"buy_now",
             *        "subscription?":false,
             *        "repeat":null,
             *        "style":"custom_large",
             *        "text":"Pay With Bitcoin",
             *        "name":"Order Name",
             *        "description":"Order Description",
             *        "custom":"Custom_Order_Id",
             *        "callback_url":"http://www.bitarmory.com/callback",
             *        "success_url":"http://www.bitarmory.com/success",
             *        "cancel_url":"http://www.bitarmory.com/cancel",
             *        "info_url":"http://www.bitarmory.com/info",
             *        "auto_redirect":false,"auto_redirect_success":false,
             *        "auto_redirect_cancel":false,
             *        "price":
             *            {"cents":7999.0,
             *            "currency_iso":"USD"
             *            },
             *        "variable_price":false,
             *        "choose_price":false,
             *        "include_address":false,
             *        "include_email":false
             *        }
             *    }
             */
            var json = @"{""success"":false,""errors"":[""You have not filled out your merchant profile. Please enter your information in the Profile section.""],""button"":{""code"":""a0f012205616bd374e16d3afddb7c2ae"",""type"":""buy_now"",""subscription?"":false,""repeat"":null,""style"":""custom_large"",""text"":""Pay With Bitcoin"",""name"":""Order Name"",""description"":""Order Description"",""custom"":""Custom_Order_Id"",""callback_url"":""http://www.bitarmory.com/callback"",""success_url"":""http://www.bitarmory.com/success"",""cancel_url"":""http://www.bitarmory.com/cancel"",""info_url"":""http://www.bitarmory.com/info"",""auto_redirect"":false,""auto_redirect_success"":false,""auto_redirect_cancel"":false,""price"":{""cents"":7999.0,""currency_iso"":""USD""},""variable_price"":false,""choose_price"":false,""include_address"":false,""include_email"":false}}";

            var b = JsonConvert.DeserializeObject<ButtonResponse>(json);
        }





        [Test]
        public void test_callback()
        {
            var json = @"{
  ""order"": {
    ""id"": ""5RTQNACF"",
    ""created_at"": ""2012-12-09T21:23:41-08:00"",
    ""status"": ""completed"",
    ""total_btc"": {
      ""cents"": 100000000,
      ""currency_iso"": ""BTC""
    },
    ""total_native"": {
      ""cents"": 1253,
      ""currency_iso"": ""USD""
    },
    ""custom"": ""order1234"",
    ""receive_address"": ""1NhwPYPgoPwr5hynRAsto5ZgEcw1LzM3My"",
    ""button"": {
      ""type"": ""buy_now"",
      ""name"": ""Alpaca Socks"",
      ""description"": ""The ultimate in lightweight footwear"",
      ""id"": ""5d37a3b61914d6d0ad15b5135d80c19f""
    },
    ""transaction"": {
      ""id"": ""514f18b7a5ea3d630a00000f"",
      ""hash"": ""4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b"",
      ""confirmations"": 0
    },
    ""customer"": {
      ""email"": ""[email protected]"",
      ""shipping_address"": [
        ""John Smith"",
        ""123 Main St."",
        ""Springfield, OR 97477"",
        ""United States""
      ]
    }
  }
}";
            var obj = JsonConvert.DeserializeObject<Mvc.CoinbaseCallback>( json );

            obj.Should().NotBeNull();

            var truth = new Mvc.CoinbaseCallback
                {
                    Order = new Order
                        {
                            Button = new ButtonDesc
                                {
                                    Type = ButtonType.BuyNow,
                                    Name = "Alpaca Socks",
                                    Description = "The ultimate in lightweight footwear",
                                    Id = "5d37a3b61914d6d0ad15b5135d80c19f"
                                },
                            CreatedAt = DateTime.Parse( "2012-12-09T21:23:41-08:00" ),
                            Status = Status.Completed,
                            Id = "5RTQNACF",
                            TotalBtc = new Price
                                {
                                    Cents = 100000000,
                                    Currency = Currency.BTC
                                },
                            TotalNative = new Price
                                {
                                    Cents = 1253,
                                    Currency = Currency.USD
                                },
                            Custom = "order1234",
                            ReceiveAddress = "1NhwPYPgoPwr5hynRAsto5ZgEcw1LzM3My",
                            Transaction = new Transaction
                                {
                                    Id = "514f18b7a5ea3d630a00000f",
                                    Hash = "4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b",
                                    Confirmations = 0
                                },
                            Customer = new Customer
                                {
                                    Email = "[email protected]",
                                    ShippingAddress = new[]
                                        {
                                            "John Smith",
                                            "123 Main St.",
                                            "Springfield, OR 97477",
                                            "United States"
                                        }
                                }
                        }

                };


            obj.ShouldBeEquivalentTo( truth );
        }

        [Test]
        public void should_be_able_to_parse_sell_response()
        {
            var json = @"{
  ""success"": true,
  ""transfer"": {
    ""id"": ""5456c2cb46cd93593d00000b"",
    ""type"": ""Sell"",
    ""code"": ""5456c2cb46cd93593d00000b"",
    ""created_at"": ""2013-01-28T16:32:35-08:00"",
    ""fees"": {
      ""coinbase"": {
        ""cents"": 14,
        ""currency_iso"": ""USD""
      },
      ""bank"": {
        ""cents"": 15,
        ""currency_iso"": ""USD""
      }
    },
    ""status"": ""Created"",
    ""payout_date"": ""2013-02-01T18:00:00-08:00"",
    ""btc"": {
      ""amount"": ""1.00000000"",
      ""currency"": ""BTC""
    },
    ""subtotal"": {
      ""amount"": ""13.50"",
      ""currency"": ""USD""
    },
    ""total"": {
      ""amount"": ""13.21"",
      ""currency"": ""USD""
    }
  }
}";

            var obj = JsonConvert.DeserializeObject<SellResponse>(json);

            obj.Should().NotBeNull();
            
            var truth = new SellResponse()
                {
                    Success = true,
                    Transfer = new TransferDesc
                        {
                            Id = "5456c2cb46cd93593d00000b",
                            type = "Sell",
                            Code = "5456c2cb46cd93593d00000b",
                            CreatedAt = DateTime.Parse("2013-01-28T16:32:35-08:00"),
                            Fees = new Fees
                                {
                                    Coinbase = new Price {Cents = 14, Currency = Currency.USD},
                                    Bank = new Price {Cents = 15, Currency = Currency.USD}
                                },
                            Status = Status.Created,
                            PayoutDate = DateTime.Parse("2013-02-01T18:00:00-08:00"),
                            Btc = new MoneyAmount
                                {
                                    Amount = 1.00000000m,
                                    Currency = Currency.BTC,
                                },
                            Subtotal = new MoneyAmount
                                {
                                    Amount = 13.50m,
                                    Currency = Currency.USD
                                },
                            Total = new MoneyAmount
                                {
                                    Amount = 13.21m,
                                    Currency = Currency.USD
                                }
                        }
                };

            obj.ShouldBeEquivalentTo(truth);
        }
    }

   
}