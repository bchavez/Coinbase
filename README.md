[![Build status](https://ci.appveyor.com/api/projects/status/t6j3xe6cr0mu8si5?svg=true)](https://ci.appveyor.com/project/bchavez/coinbase) [![Nuget](https://img.shields.io/nuget/v/Coinbase.svg)](https://www.nuget.org/packages/Coinbase/) [![Users](https://img.shields.io/nuget/dt/Coinbase.svg)](https://www.nuget.org/packages/Coinbase/)
Coinbase .NET/C# Library
======================

Project Description
-------------------
A .NET implementation for the [Coinbase API](https://developers.coinbase.com/api/v2). This library uses API version 2.

### License
* [MIT License](https://github.com/bchavez/Dwolla/blob/master/LICENSE)

### Requirements
* .NET 4.0

### Download & Install
Nuget Package **[Coinbase](https://www.nuget.org/packages/Coinbase/)**

```

Install-Package Coinbase

```

Building
--------
* Download the source code.
* Run `build.cmd`.

Upon successful build, the results will be in the `\__compile` directory. If you want to build NuGet packages, run `build.cmd pack` and the NuGet packages will be in `__package`.


Usage
-----
### API Authentication
Coinbase offers two ways to authenticate your application with their API:

* By using an [**API key**](https://coinbase.com/docs/api/authentication#api_key).
* By using [**OAuth2**](https://coinbase.com/docs/api/authentication#oauth2).

This library uses the **API key** authentication make calls to Coinbase servers.

----
###Integration Options
#### Redirect to Payment Page
This integration option redirects the user's browser to Coinbase's servers to complete the checkout / payment process. Optionally, a **CallbackUrl** and/or **SuccessUrl** can be added to the payment that will notify a server on the success or failure of a payment.

This integration method is similar [PayPal's Express Checkout (web checkout)](https://developer.paypal.com/webapps/developer/docs/integration/web/web-checkout/) or [Dwolla's off-site gateway](https://developers.dwolla.com/dev/pages/gateway#ux) payment method.

When a customer has finished selecting items they wish to purchase, the checkout payment process is completed by redirecting the user's browser to Coinbase's servers where the user is presented with the following checkout page:

![Payment Page](https://raw.github.com/bchavez/Coinbase/50f55472be49008af2cdda7959284a304af6ca78/Docs/payaddress.png)

Once the customer sends **Bitcoins** to the **Bitcoin Address**, the user then clicks on **Confirm Payment**. Payment confirmation will verify that the funds have been transferred to the **Bitcoin Address** that is associated with your merchant account.

![Payment Confirm](https://raw.github.com/bchavez/Coinbase/50f55472be49008af2cdda7959284a304af6ca78/Docs/payaddress-wait.png)

When the transfer of **Bitcoins** has been verified, the user is redirected to the **SuccessUrl** and optionally the **CallbackUrl** is invoked.

**Note:** A separate **Bitcoin Address** is generated for each order and customer. This ensures order totals don't build up at a single **Bitcoin Address**.

**Note:** Once a payment page is created, the customer will have about 10 minutes to complete their purchase. After 10 minutes the URL to the payment page will expire.

The following server code registers a payment request with Coinbase and retrieves a checkout redirect URL for the user's browser:

```csharp
var api = new CoinbaseApi( apiKey: "my_api_key", apiSecret: "my_api_secret" );

var checkout = api.CreateCheckout(new CheckoutRequest
    {
        Amount = 10.00m,
        Currency = "USD",
        Name = "Test Order",
        NotificationsUrl = ""
    });

if( !checkout.Errors.Any() )
{
    var redirectUrl = api.GetCheckoutUrl(checkout);
    //do redirect with redirectUrl
}
else
{
    //Error making checkout page.
}
```

-------
#### Handling Callbacks on Your Server

Verifying callback authenticity is non-existent in Coinbase's API at the time of this writing. Therefore, it is necessary to implement some kind of security mechanism to verify the authenticity of callbacks. Using [HMAC](http://en.wikipedia.org/wiki/Hash-based_message_authentication_code) or any other message authentication should be used to verify that the original payment request is indeed authentic and originated from Coinbase. Usually, choosing a secret that only you and Coinbase know is a good start.

To do this, (and for sake of simplicity), a `Guid` is used to record each checkout. The `Custom` property in the button request is used to identify the order pending inside a database.

For example, here's a simple order that we will handle our callback on:

```csharp
// CREATE THE ORDER, AND REDIRECT
var api = new CoinbaseApi( apiKey: "my_api_key" );

var purchaseId = Guid.NewGuid().ToString("n");

var checkoutRequest = new CheckoutRequest
    {
        Name = "Best Candy Bar on Earth",
        Currency = Currency.USD,
        Price = 79.99m,
        Type = ButtonType.BuyNow,
        Custom = purchaseId, //<< Here is how we identify the order, our purchaseId
        Description = "Yummy Candy bar",
        Style = ButtonStyle.CustomLarge,
        CallbackUrl = "http://domain.com/bitcoin/callback"
        SuccessUrl = "http://domain.com/bitcoin/success"
    };

var checkout = api.CreateCheckout( checkoutRequest );

if ( !checkout.Errors.Any() )
{
    var redirectUrl = api.GetCheckoutUrl(checkout);
    Redirect(redirectUrl);
}
```
It's important to note that we're setting a `CallbackUrl` and a `SuccessUrl`. The `CallbackUrl` will be invoked **asynchronously** after the customer has completed their purchase. The `SuccessUrl` is invoked **synchronously** by the customer's browser after the customer has completed their payment transaction.

An MVC controller action that handles the callback **asynchronously** might look like this:

```csharp
//This method is invoked by Coinbase's servers.
[Route( "bitcoin/callback" ), HttpPost]
public ActionResult Bitcoin_Execute()
{
    Notification notification = null;
    using( var sr = new StreamReader(Request.InputStream) )
    {
        notification = api.GetNotification(sr.ReadToEnd());
    }
 
    var order = notification.UnverifiedOrder;

    // Verify the order came from Coinbase servers
    if( valid ){
        //process the order callback
		return new HttpStatusCodeResult( HttpStatusCode.OK );
    }

    //else, bad request.
    return new HttpStatusCodeResult( HttpStatusCode.BadRequest );
}

```

With **Web API v2**, it's slightly eaiser:
```
[Route( "bitcoin/callback" ), HttpPost]
public IHttpActionResult Bitcoin_Execute(Notification notification)
{
    var order = notification.UnverifiedOrder;

    // Verify the order came from Coinbase servers
    if( valid ){
        //process the order callback
        return new HttpResponseMessage( HttpStatusCode.OK )
    }

    //else, bad request.
    return new HttpResponseMessage( HttpStatusCode.BadRequest );
}
```


-------
#### Handling Redirects on Your Server
An MVC controller action that handles the customer's `SuccessUrl` redirect might look like this:

```csharp
//This action is invoked by the customer's browser
//and after successfully completing their payment
//This handles the redirect back to the merchant's website.
[Route( "bitcoin/success" ), HttpGet]
public ActionResult Bitcoin_GetRequest()
{
    if ( this.Request.QueryString["order[status]"].StartsWith( "complete", StringComparison.OrdinalIgnoreCase ) )
    {
        var purchaseId = this.Request.QueryString["order[custom]"];

        //The bitcoin payment has completed, use the purchaseId
        //to fulfill the order.
    }

    //Show Error.
}
```

-------
#### Raw API call example using SendRequest
The example below demonstrates using `SendRequest` to [refund](https://developers.coinbase.com/api/v2#refund-an-order) an order:

```csharp

var api = new CoinbaseApi(apiKey:"my_api_key", apiSecret:"my_api_secret");

var options = new
    {
        currency = "BTC"
    };

var orderId = "ORDER_ID_HERE";

var response = api.SendRequest($"/orders/{orderId}/refund", options);
//process response as needed
```

See the Coinbase docs for more complete parameter [information](https://developers.coinbase.com/api/v2#refund-an-order).

Other API calls follow the same pattern of using anonymous types as request body parameters. For the complete list of API calls available, please see the main documentation page [here](https://developers.coinbase.com/api/v2).


Reference
---------
* [Coinbase API Documentation](https://developers.coinbase.com/api/v2)


Contributors
---------
Created by [Brian Chavez](http://bchavez.bitarmory.com).

A big thanks to GitHub and all contributors:

* [ElanHasson](https://github.com/ElanHasson) (Elan Hasson)
* [ryanmwilliams](https://github.com/ryanmwilliams) (Ryan Williams)

---

*Note: This application/third-party library is not directly supported by Coinbase Inc. Coinbase Inc. makes no claims about this application/third-party library.  This application/third-party library is not endorsed or certified by Coinbase Inc.*
