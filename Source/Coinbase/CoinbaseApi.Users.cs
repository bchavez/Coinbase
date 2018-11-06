﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl;
using Flurl.Http;

namespace Coinbase
{
   public interface IUsersEndpoint
   {
      /// <summary>
      /// Get any user’s public information with their ID.
      /// </summary>
      Task<Response<User>> GetUserAsync(string userId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Get current user’s public information. To get user’s email or private information, use permissions wallet:user:email and wallet:user:read. If current request has a wallet:transactions:send scope, then the response will contain a boolean sends_disabled field that indicates if the user’s send functionality has been disabled.
      /// </summary>
      Task<Response<User>> GetCurrentUserAsync(CancellationToken cancellationToken = default);

      /// <summary>
      /// Get current user’s authorization information including granted scopes and send limits when using OAuth2 authentication.
      /// </summary>
      Task<Response<Auth>> GetAuthInfoAsync(CancellationToken cancellationToken = default);

      /// <summary>
      /// Modify current user and their preferences.
      /// </summary>
      Task<Response<User>> UpdateUserAsync(UserUpdate update, CancellationToken cancellationToken = default);
   }


   public partial class CoinbaseApi : IUsersEndpoint
   {
      public IUsersEndpoint Users => this;

      /// <summary>
      /// Get any user’s public information with their ID.
      /// </summary>
      Task<Response<User>> IUsersEndpoint.GetUserAsync(string userId, CancellationToken cancellationToken = default)
      {
         if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

         return this.apiUrl
            .AppendPathSegments("users",userId)
            .WithClient(this.client)
            .GetJsonAsync<Response<User>>(cancellationToken);
      }
      /// <summary>
      /// Get current user’s public information. To get user’s email or private information, use permissions wallet:user:email and wallet:user:read. If current request has a wallet:transactions:send scope, then the response will contain a boolean sends_disabled field that indicates if the user’s send functionality has been disabled.
      /// </summary>
      Task<Response<User>> IUsersEndpoint.GetCurrentUserAsync(CancellationToken cancellationToken = default)
      {
         return this.apiUrl
            .AppendPathSegment("user")
            .WithClient(this.client)
            .GetJsonAsync<Response<User>>(cancellationToken);
      }
      /// <summary>
      /// Get current user’s authorization information including granted scopes and send limits when using OAuth2 authentication.
      /// </summary>
      Task<Response<Auth>> IUsersEndpoint.GetAuthInfoAsync(CancellationToken cancellationToken = default)
      {
         return this.apiUrl
            .AppendPathSegments("user", "auth")
            .WithClient(this.client)
            .GetJsonAsync<Response<Auth>>(cancellationToken);
      }
      /// <summary>
      /// Modify current user and their preferences.
      /// </summary>
      Task<Response<User>> IUsersEndpoint.UpdateUserAsync(UserUpdate update, CancellationToken cancellationToken = default)
      {
         return this.apiUrl
            .AppendPathSegment("user")
            .WithClient(this.client)
            .PutJsonAsync(update, cancellationToken)
            .ReceiveJson<Response<User>>();
      }
   }
}