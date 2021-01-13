using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl;
using Flurl.Http;

namespace Coinbase
{
   /// <summary>
   /// https://developers.coinbase.com/docs/wallet/coinbase-connect/integrating
   /// https://developers.coinbase.com/docs/wallet/coinbase-connect/reference
   /// </summary>
   public class AuthorizeOptions
   {
      /// <summary>
      /// Required The client ID you received after registering your application.
      /// </summary>
      public string ClientId { get; set; }

      /// <summary>
      /// Optional The URL in your app where users will be sent after authorization (see below). This value needs to be URL encoded. If left out, your application’s first redirect URI will be used by default.
      /// </summary>
      public string RedirectUri { get; set; }

      /// <summary>
      /// Optional An unguessable random string. It is used to protect against cross-site request forgery attacks.
      /// </summary>
      public string State { get; set; }

      /// <summary>
      /// Optional Comma separated list of permissions (scopes) your application requests access to. Required scopes are listed under endpoints in the Full Scopes List
      /// </summary>
      public string Scope { get; set; }

      /// <summary>
      /// For logged out users, login view is shown by default. You can show the sign up page instead with value signup
      /// </summary>
      public string Layout { get; set; }
      /// <summary>
      /// Earn a referral bonus from new users who sign up via OAuth. Value needs to be set to developer’s referral ID (username). Read more.
      /// </summary>
      public string Referral { get; set; }

      /// <summary>
      /// Change the account access the application will receive. Available values:
      /// * 'select' (default) Allow user to pick the wallet associated with the application
      /// * 'all' Application will get access to all of user’s wallets
      /// For backward compatibility all is used as default for applications created prior to this change
      /// </summary>
      public string Account { get; set; }

      public IDictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();

      /// <summary>
      /// Name for this session (not a name for your application.) This will appear in the user’s account settings underneath your application’s name. Use it to provide identifying information if your app is often authorized multiple times
      /// </summary>
      public string Name
      {
         set => this.Meta["name"] = value;
      }

      public decimal SendLimitAmount
      {
         set => this.Meta["send_limit_amount"] = value;
      }

      public string SendLimitCurrency
      {
         set => this.Meta["send_limit_currency"] = value;
      }

      public string SendLimitPeriod
      {
         set => this.Meta["send_limit_period"] = value;
      }
   }

   public static class OAuthHelper
   {
      public const string OAuthEndpoint = "https://api.coinbase.com/oauth";

      public const string AuthorizeEndpoint = "https://www.coinbase.com/oauth/authorize";

      /// <summary>
      /// When redirecting a user to Coinbase to authorize access to your application,
      /// you’ll need to construct the authorization URL with the correct parameters
      /// and scopes. Here’s a list of parameters you should always specify:
      /// </summary>
      public static string GetAuthorizeUrl(AuthorizeOptions opts)
      {
         var url = AuthorizeEndpoint
            .SetQueryParam("response_type", "code")
            .SetQueryParam("client_id", opts.ClientId)
            .SetQueryParam("redirect_uri", opts.RedirectUri)
            .SetQueryParam("state", opts.State)
            .SetQueryParam("scope", opts.Scope)
            .SetQueryParam("layout", opts.Layout)
            .SetQueryParam("referral", opts.Referral)
            .SetQueryParam("account", opts.Account);

         foreach( var kv in opts.Meta )
         {
            url.SetQueryParam($"meta[{kv.Key}]", kv.Value);
         }

         return url;
      }

      /// <summary>
      /// After you have received the temporary code, you can exchange it for valid
      /// access and refresh tokens.
      /// </summary>
      /// <param name="code">Required Value from the GetAuthorizeUrl step.</param>
      /// <param name="clientId">Required The client ID you received after registering your application.</param>
      /// <param name="clientSecret">Required The client secret you received after registering your application.</param>
      /// <param name="redirectUri">Required Your application’s redirect URI</param>
      public static Task<OAuthResponse> GetAccessTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
      {
         var form = new
            {
               grant_type = "authorization_code",
               code,
               client_id = clientId,
               client_secret = clientSecret,
               redirect_uri = redirectUri
            };

         return OAuthEndpoint
            .AppendPathSegment("token")
            .PostUrlEncodedAsync(form)
            .ReceiveJson<OAuthResponse>();
      }

      /// <summary>
      /// Coinbase uses an optional security feature of OAuth2 called refresh tokens.
      /// When you first authenticate, your app will be given an access_token and a
      /// refresh_token. The access token is used to authenticate all your requests,
      /// but expires in two hours. Once an access token has expired, you will need
      /// to use the refresh token to obtain a new access token and a new refresh token.
      /// The refresh token never expires but it can only be exchanged once for a new
      /// set of access and refresh tokens. If you try to make a call with an expired
      /// access token, a 401 response will be returned.
      /// </summary>
      /// <param name="refreshToken"></param>
      /// <param name="clientId"></param>
      /// <param name="clientSecret"></param>
      /// <returns></returns>
      public static Task<OAuthResponse> RenewAccessAsync(string refreshToken, string clientId, string clientSecret)
      {
         var form = new
            {
               grant_type = "refresh_token",
               refresh_token = refreshToken,
               client_id = clientId,
               client_secret = clientSecret
            };
         return OAuthEndpoint
            .AppendPathSegment("token")
            .PostUrlEncodedAsync(form)
            .ReceiveJson<OAuthResponse>();
      }

      /// <summary>
      /// Active access tokens can be revoked at any time. This request needs to be made authenticated like any other regular API request (either containing access_token parameter or Authentication header with bearer token) and 200 OK is returned for both successful and unsuccessful request. This can be useful, for example, when implementing log-out feature.
      /// </summary>
      /// <remarks>
      /// Access tokens can be revoked manually if you want to disconnect your application’s access to the user’s account. Revoking can also be used to implement a log-out feature. You’ll need to supply the current access token twice, once to revoke it, and another to authenticate the request (either containing access_token parameter or Authentication header with bearer token). 200 OK is returned for both successful and unsuccessful requests.
      /// </remarks>
      /// <param name="token">The access token to expire.</param>
      /// <param name="accessToken">The token used to make the authenticated request. This can be the same as the token in the first parameter.</param>
      public static Task<IFlurlResponse> RevokeTokenAsync(string token, string accessToken)
      {
         var form = new
            {
               token,
               access_token = accessToken
            };
         return OAuthEndpoint
            .AppendPathSegment("revoke")
            .PostUrlEncodedAsync(form);
      }
   }

   public static class AutoRefreshTokenHelper
   {
      public const string ExpiredToken = "expired_token";

      /// <summary>
      /// Setup the CoinbaseClient to use automatic token refresh.
      /// </summary>
      /// <param name="clientId">The OAuth Application Client ID</param>
      /// <param name="clientSecret">The OAuth Application Secret</param>
      /// <param name="onRefresh">Callback function to invoke when the OAuth token is refreshed.</param>
      public static CoinbaseClient WithAutomaticOAuthTokenRefresh(this CoinbaseClient client, string clientId, string clientSecret, Func<OAuthResponse, Task> onRefresh = null)
      {
         if( client.Config is OAuthConfig config )
         {
         }
         else throw new InvalidOperationException($"Client must be using an {nameof(OAuthConfig)}");

         async Task TokenExpiredErrorHandler(FlurlCall call)
         {
            var exception = call.Exception;
            if (exception is FlurlHttpException ex)
            {
               var errorResponse = await ex.GetResponseJsonAsync<JsonResponse>().ConfigureAwait(false);
               if (errorResponse.Errors.Any(x => x.Id == ExpiredToken))
               {
                  var refreshResponse = await OAuthHelper.RenewAccessAsync(config.RefreshToken, clientId, clientSecret).ConfigureAwait(false);
                  config.AccessToken = refreshResponse.AccessToken;
                  config.RefreshToken = refreshResponse.RefreshToken;

                  if( onRefresh is null )
                  {
                  }
                  else await onRefresh(refreshResponse).ConfigureAwait(false);

                  call.Response = await call.Request.SendAsync(call.Request.Verb, call.HttpRequestMessage.Content).ConfigureAwait(false);
                  call.ExceptionHandled = true;
               }
            }
         }

         client.Configure(s => s.OnErrorAsync = TokenExpiredErrorHandler);
         return client;
      }
   }
}
