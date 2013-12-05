Coinbase .NET/C# Library
======================
----------------------

Project Description
-------------------
A .NET implementation for the [Coinbase API](https://coinbase.com/docs/api/overview)

### License
* [MIT License](https://github.com/bchavez/Dwolla/blob/master/LICENSE)

### Requirements
* .NET 4.0

### Download & Install
Nuget Package [Coinbase](https://www.nuget.org/packages/Coinbase/)

```

Install-Package Coinbase

```

Building
--------
* Download the source code.
* Run `build.bat`.

Upon successful build, the results will be in the `\__package` directory.


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
var api = new CoinbaseApi( apiKey: "my_api_key" );

var paymenRequest = new ButtonRequest
    {
        Name = "Order Name",
        Currency = Currency.USD,
        Price = 79.99m,
        Type = ButtonType.BuyNow,
        Custom = "Custom_Order_Id",
        Description = "Order Description",
        Style = ButtonStyle.CustomLarge,
    };

var buttonResponse = api.RegisterButton( paymenRequest );

if ( buttonResponse.Success )
{
    var redirectUrl = buttonResponse.GetCheckoutUrl();
    //Redirect the user to the URL to complete the
    //the purchase
}
else
{
    //Something went wrong. Check errors and fix any issues.
    Debug.WriteLine( string.Join( ",", buttonResponse.Errors ) );
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

var paymenRequest = new ButtonRequest
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

var buttonResponse = api.RegisterButton( paymenRequest );

if ( buttonResponse.Success )
{
    var redirectUrl = buttonResponse.GetCheckoutUrl();
    return Redirect
}
```
It's important to note that we're setting a `CallbackUrl` and a `SuccessUrl`. The `CallbackUrl` will be invoked **asynchronously** after the customer has completed their purchase. The `SuccessUrl` is invoked **synchronously** by the customer's browser after the customer has completed their payment transaction.

An MVC controller action that handles the callback **asynchronously** might look like this:

```csharp
//This method is invoked by Coinbase's servers.
[Route( "bitcoin/callback" ), HttpPost]
public ActionResult Bitcoin_Execute( [JsonNetBinder] CoinbaseCallback callback )
{
    if( callback.Order.Status == Status.Completed )
    {
        var purchaseId = callback.Order.Custom;
        
        //check DB for purchaseId Guid

        return new HttpStatusCodeResult( HttpStatusCode.OK );
    }
    return new HttpStatusCodeResult( HttpStatusCode.BadRequest );
}

```
**Note:** Don't forget the `[JsonNetBinder]` attribute on the callback parameter. It's needed to fully parse the callback with `Newtonsoft.Json`. Otherwise, some fields in the callback such as `CompletedAt` might be null.


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
 

Reference
---------
* [Coinbase API Documentation](https://coinbase.com/docs/api/overview)


Created by [Brian Chavez](http://bchavez.bitarmory.com).

---

*Note: This application/third-party library is not directly supported by Coinbase Inc. Coinbase Inc. makes no claims about this application/third-party library.  This application/third-party library is not endorsed or certified by Coinbase Inc.*
