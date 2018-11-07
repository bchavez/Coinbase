using System.Net.Http;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Coinbase.Tests.Endpoints
{
   public class UserTests : OAuthServerTest
   {
      [Test]
      public async Task can_get_user()
      {
         SetupServerSingleResponse(Examples.User);

         const string userId = "9da7a204-544e-5fd1-9a12-61176c5d4cd8";

         var user = await client.Users.GetUserAsync(userId);

         var truth = new Response<User>
            {
               Data = Examples.UserModel
            };

         user.Should().BeEquivalentTo(truth);

         server.ShouldHaveExactCall($"https://api.coinbase.com/v2/users/{userId}")
            .WithVerb(HttpMethod.Get);
      }

      [Test]
      public async Task can_get_current_user()
      {
         SetupServerSingleResponse(Examples.User);

         var user = await client.Users.GetCurrentUserAsync();

         var truth = new Response<User>
            {
               Data = Examples.UserModel
            };

         user.Should().BeEquivalentTo(truth);

         server.ShouldHaveExactCall($"https://api.coinbase.com/v2/user")
            .WithVerb(HttpMethod.Get);
      }

      [Test]
      public async Task can_get_auth_information()
      {
         SetupServerSingleResponse(Examples.Auth);

         var user = await client.Users.GetAuthInfoAsync();

         var truth = new Response<Auth>
            {
               Data = Examples.AuthModel
            };

         user.Should().BeEquivalentTo(truth);

         server.ShouldHaveExactCall($"https://api.coinbase.com/v2/user/auth")
            .WithVerb(HttpMethod.Get);
      }

      [Test]
      public async Task update_user()
      {
         SetupServerSingleResponse(Examples.UserWithNameChange("bbb ccc"));

         var user = await client.Users.UpdateUserAsync(new UserUpdate{ Name = "bbb ccc"});

         var truth = new Response<User>
            {
               Data = Examples.UserModel
            };
         truth.Data.Name = "bbb ccc";

         user.Should().BeEquivalentTo(truth);

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/user")
            .WithVerb(HttpMethod.Put);
      }


   }
}