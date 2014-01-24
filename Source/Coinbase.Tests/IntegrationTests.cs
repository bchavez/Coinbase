using System;
using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace Coinbase.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        [Explicit]
        public void integration_test_create_button()
        {
            var api = new CoinbaseApi();

            var paymenRequest = new ButtonRequest
                {
                    Name = "Order Name",
                    Currency = Currency.USD,
                    Price = 79.99m,
                    Type = ButtonType.BuyNow,
                    Custom = "Custom_Order_Id",
                    Description = "Order Description",
                    Style = ButtonStyle.CustomLarge,
                    CallbackUrl = "http://www.bitarmory.com/callback",
                    CancelUrl = "http://www.bitarmory.com/cancel",
                    InfoUrl = "http://www.bitarmory.com/info",
                    SuccessUrl = "http://www.bitarmory.com/success",
                    ChoosePrice = false,
                    IncludeAddress = false,
                    IncludeEmail = false,
                    VariablePrice = false
                };

            var buttonResponse = api.RegisterButton( paymenRequest );

            if( buttonResponse.Success )
            {
                var redirectUrl = buttonResponse.GetCheckoutUrl();
                //Redirect the user to the URL to complete the
                //the purchase
                Console.WriteLine( redirectUrl );
            }
            else
            {
                //Something went wrong. Check errors and fix any issues.
                Debug.WriteLine( string.Join( ",", buttonResponse.Errors ) );
            }
            
            var o = api.CreateOrder( buttonResponse );
            o.Should().NotBeNull();

            buttonResponse.Button.Code.Should().NotBeNullOrEmpty();

            buttonResponse.Button.Price.Cents.Should().Be( 7999 );
                    
            buttonResponse.Success.Should().BeTrue();
            //http://www.bitarmory.com/cancel?order%5Bbutton%5D%5Bdescription%5D=Order+Description&order%5Bbutton%5D%5Bid%5D=ea607b144c6fc28ec289eea5acaaaf86&order%5Bbutton%5D%5Bname%5D=Order+Name&order%5Bbutton%5D%5Btype%5D=buy_now&order%5Bcreated_at%5D=2013-12-01+19%3A04%3A24+-0800&order%5Bcustom%5D=Custom_Order_Id&order%5Bid%5D=3LP5XUP7&order%5Breceive_address%5D=13uwaYfphxs51eN2DuhBJqimRJJ3UrYjSX&order%5Bstatus%5D=new&order%5Btotal_btc%5D%5Bcents%5D=7767029&order%5Btotal_btc%5D%5Bcurrency_iso%5D=BTC&order%5Btotal_native%5D%5Bcents%5D=7999&order%5Btotal_native%5D%5Bcurrency_iso%5D=USD&order%5Btransaction%5D=
            //http://www.bitarmory.com/cancel?
            //order[button][description]=Order Description
            //&order[button][id]=ea607b144c6fc28ec289eea5acaaaf86
            //&order[button][name]=Order Name
            //&order[button][type]=buy_now&order[created_at]=2013-12-01 19:04:24 -0800
            //&order[custom]=Custom_Order_Id
            //&order[id]=3LP5XUP7
            //&order[receive_address]=13uwaYfphxs51eN2DuhBJqimRJJ3UrYjSX
            //&order[status]=new&order[total_btc][cents]=7767029
            //&order[total_btc][currency_iso]=BTC
            //&order[total_native][cents]=7999
            //&order[total_native][currency_iso]=USD
            //&order[transaction]=
        }
    }
}