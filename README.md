[![Build status](https://ci.appveyor.com/api/projects/status/t6j3xe6cr0mu8si5?svg=true)](https://ci.appveyor.com/project/bchavez/coinbase) [![Nuget](https://img.shields.io/nuget/v/Coinbase.svg)](https://www.nuget.org/packages/Coinbase/) [![Users](https://img.shields.io/nuget/dt/Coinbase.svg)](https://www.nuget.org/packages/Coinbase/) <img src="https://raw.githubusercontent.com/bchavez/Coinbase/master/Docs/coinbase.png" align='right' />

Coinbase .NET/C# Library
======================

Project Description
-------------------
A .NET implementation for the [Coinbase API](https://developers.coinbase.com/api/v2). This library uses API version 2.

:loudspeaker: ***HEY!*** If you're looking for the [**Coinbase Commerce** API, check this link!](https://github.com/bchavez/Coinbase.Commerce)

#### Supported Platforms
* **.NET Standard 2.0** or later
* **.NET Framework 4.5** or later

#### Crypto Tip Jar
<a href="https://commerce.coinbase.com/checkout/119320d7-db0a-45c9-97d8-fe4088348288"><img src="https://raw.githubusercontent.com/bchavez/Coinbase/master/Docs/tipjar.png" /></a>
* :dog2: **Dogecoin**: `DGVC2drEMt41sEzEHSsiE3VTrgsQxGn5qe`



### Download & Install
**Nuget Package [Coinbase](https://www.nuget.org/packages/Coinbase/)**

```powershell
Install-Package Coinbase
```

Usage
-----
### API Authentication
Coinbase offers two ways to authenticate your application with their API:

* By using [**OAuth2**](https://developers.coinbase.com/api/v2#oauth2-coinbase-connect).
* By using an [**API key + Secret**](https://developers.coinbase.com/api/v2#api-key).

This library can use both **OAuth** and **API Key + Secret** authentication make calls to Coinbase servers.

----
### Getting Started

For the most part, to get the started, simply new up a new `CoinbaseApi` object as shown below:
```csharp
//using OAuth Token
var api = new CoinbaseApi(new Config{ OAuthToken = "..." });

//using API Key + Secret
var api = new CoinbaseApi(new Config{ ApiKey = "...", ApiSecret = "..."});

//No authentication
//  - Useful only for Data Endpoints that don't require authentication.
var api = new CoinbaseApi();
```
Once you have a `CoinbaseApi` object, simply call one of any of the [**Wallet Endpoints**](https://developers.coinbase.com/api/v2#wallet-endpoints) or [**Data Endpoints**](https://developers.coinbase.com/api/v2#data-endpoints). Extensive examples can be [found here](https://github.com/bchavez/Coinbase/tree/master/Source/Coinbase.Tests/Endpoints).

In one such example, to get the [spot price](https://developers.coinbase.com/api/v2#get-spot-price) of `ETH-USD`, do the following:
```csharp
[Test]
public async Task can_get_spotprice_of_ETHUSD()
{
   var spot = await api.GetSpotPriceAsync("ETH-USD");
   spot.Data.Amount.Should().BeGreaterThan(5);
   spot.Data.Currency.Should().Be("USD");
   spot.Data.Base.Should().Be("ETH");
}
``` 

-------
#### Handling Callback Notifications on Your Server

This library can handle verification of notifications / webhook / callbacks from Coinbase. When an intresting event occurs, Coinbase can `POST` **JSON** notifications to your server. When receiving a notification, an **HTTP** `POST` is delivered to your server that looks similar to:

```
POST /callmebackhere HTTP/1.1
User-Agent: weipay-webhooks
Content-Type: application/json
CB-SIGNATURE: 6yQRl17CNj5YSHSpF+tLjb0vVsNVEv021Tyy1bTVEQ69SWlmhwmJYuMc7jiDyeW9TLy4vRqSh4g4YEyN8eoQIM57pMoNw6Lw6Oudubqwp+E3cKtLFxW0l18db3Z/vhxn5BScAutHWwT/XrmkCNaHyCsvOOGMekwrNO7mxX9QIx21FBaEejJeviSYrF8bG6MbmFEs2VGKSybf9YrElR8BxxNe/uNfCXN3P5tO8MgR5wlL3Kr4yq8e6i4WWJgD08IVTnrSnoZR6v8JkPA+fn7I0M6cy0Xzw3BRMJAvdQB97wkobu97gFqJFKsOH2u/JR1S/UNP26vL0mzuAVuKAUwlRn0SUhWEAgcM3X0UCtWLYfCIb5QqrSHwlp7lwOkVnFt329Mrpjy+jAfYYSRqzIsw4ZsRRVauy/v3CvmjPI9sUKiJ5l1FSgkpK2lkjhFgKB3WaYZWy9ZfIAI9bDyG8vSTT7IDurlUhyTweDqVNlYUsO6jaUa4KmSpg1o9eIeHxm0XBQ2c0Lv/T39KNc/VOAi1LBfPiQYMXD1e/8VuPPBTDGgzOMD3i334ppSr36+8YtApAn3D36Hr9jqAfFrugM7uPecjCGuleWsHFyNnJErT0/amIt24Nh1GoiESEq42o7Co4wZieKZ+/yeAlIUErJzK41ACVGmTnGoDUwEBXxADOdA=
Accept-Encoding: gzip;q=1.0,deflate;q=0.6,identity;q=0.3
Accept: */*
Connection: close
Content-Length: 1122

{"order":{"id":null,"created_at":null,"status":"completed","event":null,"total_btc":{"cents":100000000,"currency_iso":"BTC"},"total_native":{"cents":1000,"currency_iso":"USD"},"total_payout":{"cents":1000,"currency_iso":"USD"},"custom":"123456789","receive_address":"mzVoQenSY6RTBgBUcpSBTBAvUMNgGWxgJn","button":{"type":"buy_now","name":"Test Item","description":null,"id":null},"transaction":{"id":"53bdfe4d091c0d74a7000003","hash":"4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b","confirmations":0}}}
```

The two important pieces of information you need to extract from this **HTTP** `POST` callback are:

* The `CB-SIGNATURE` header value.
* The ***raw*** **HTTP** body **JSON** payload. 

The `WebhookHelper` static class included with this library does all the heavy lifting for you. All you need to do is call `Webhookhelper.IsValid()` supplying the `CB-SIGNATURE` header value in the **HTTP** `POST` above, and the ***raw*** **JSON** body in the **HTTP** `POST` above.

The following **C#** code shows how to use the `WebhookHelper` to validate callbacks from **Coinbase**:

```csharp
if( WebhookHelper.IsValid( Request.Body.Json, cbSignatureHeaderValue) ){
   // The request is legit and an authentic message from Coinbase.
   // It's safe to deserialize the JSON body. 
   var webhook = JsonConvert.DeserializeObject<Notification>(Request.Body.Json);
   
   ... do further processing.

   return Response.Ok();
}
else {
   // Some hackery going on. The Webhook message validation failed.
   // Someone is trying to spoof payment events!
   // Log the requesting IP address and HTTP body. 
}
```

 Easy peasy! **Happy crypto shopping!** :tada:


Reference
---------
* [Coinbase API Documentation](https://developers.coinbase.com/api/v2)


Building
--------
* Download the source code.
* Run `build.cmd`.

Upon successful build, the results will be in the `\__compile` directory. If you want to build NuGet packages, run `build.cmd pack` and the NuGet packages will be in `__package`.



Contributors
---------
Created by [Brian Chavez](http://bchavez.bitarmory.com).

A big thanks to GitHub and all contributors:

* [ElanHasson](https://github.com/ElanHasson) (Elan Hasson)
* [ryanmwilliams](https://github.com/ryanmwilliams) (Ryan Williams)

---

*Note: This application/third-party library is not directly supported by Coinbase Inc. Coinbase Inc. makes no claims about this application/third-party library.  This application/third-party library is not endorsed or certified by Coinbase Inc.*
