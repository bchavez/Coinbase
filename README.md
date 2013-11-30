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
This integration option allows you to redirect the user's browser to Coinbase's servers to complete the checkout / payment process. Optionally, you can add a **CallbackUrl** and/or **SuccessUrl** that will notify your server on the success or failure of a payment.

This integration method is similar [PayPal's Express Checkout (web checkout)](https://developer.paypal.com/webapps/developer/docs/integration/web/web-checkout/) or [Dwolla's off-site gateway](https://developers.dwolla.com/dev/pages/gateway#ux) payment method.

When your customer has finished selecting their items they would like to purchase, the checkout payment process is completed by redirecting the user's browser to Coinbase's servers where the user is presented with the following checkout page:

![Payment Page](https://raw.github.com/bchavez/Coinbase/50f55472be49008af2cdda7959284a304af6ca78/Docs/payaddress.png)

Once the customer sends **Bitcoins** to the **Bitcoin Address**, the user then clicks on **Confirm Payment**. Payment confirmation will verify that the funds have been transferred to the **Bitcoin Address** that is associated with your merchant account.

![Payment Confirm](https://raw.github.com/bchavez/Coinbase/50f55472be49008af2cdda7959284a304af6ca78/Docs/payaddress-wait.png)

Upon successful transfer, the user is redirected to the **SuccessUrl** and optionally, the **CallbackUrl** is invoked.

**Note:** A separate **Bitcoin Address** is generated for each order and user. This ensures order totals don't build up at a single **Bitcoin Address**.

**Note:** Once a button page is created, the customer will have about 10 minutes to complete their purchase. If not, then the URL to the payment page will expire.

The server code to integrate the following payment page is shown below:

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

**Note:** Don't ask me why creating a payment page is called a ***button*** in their API. Coinbaise claims the underlying *data model* is the same regardless if you are creating a payment button or a payment page. It makes no sense to me to expose this detail conceptually to your API consumers; but whatever, I didn't design their API.

-------
#### Handling Callbacks on Your Server

-------
#### Handling Redirects on Your Server

 

Reference
---------
* [Coinbase API Documentation](https://coinbase.com/docs/api/overview)


Created by [Brian Chavez](http://bchavez.bitarmory.com).

---

*Note: This application/third-party library is not directly supported by Coinbase Inc. Coinbase Inc. makes no claims about this application/third-party library.  This application/third-party library is not endorsed or certified by Coinbase Inc.*
