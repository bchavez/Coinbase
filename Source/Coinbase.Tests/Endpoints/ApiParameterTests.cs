using System;
using System.Net.Http;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using Flurl.Http;
using NUnit.Framework;

namespace Coinbase.Tests.Endpoints
{
   public class ApiParameterTests : ServerTest
   {
      private CoinbaseClient client;

      [SetUp]
      public void BeforeEachTest()
      {
         client = new CoinbaseClient(new OAuthConfig {AccessToken = OAuthServerTest.OauthKey});
      }

      [Test]
      public async Task withdraw
      (
         [Values(null, "", "  ")]string accountId)
      {
         Func<Task<PagedResponse<Withdrawal>>> a = async () => await client.Withdrawals.ListWithdrawalsAsync(accountId);
         Func<Task<Response<Withdrawal>>> b = async () => await client.Withdrawals.WithdrawalFundsAsync(accountId, null);

         a.Should().Throw<ArgumentException>();
         b.Should().Throw<ArgumentException>();
      }

      [Test]
      public async Task withdraw2
      (
         [Values(null, "", "  ", "fff")]string accountId,
         [Values(null, "", "  ")]string withdrawlId)
      {
         Func<Task<Response<Withdrawal>>> a = async () => await client.Withdrawals.GetWithdrawalAsync(accountId, withdrawlId);
         Func<Task<Response<Withdrawal>>> b = async () => await client.Withdrawals.CommitWithdrawalAsync(accountId, withdrawlId);

         a.Should().Throw<ArgumentException>();
         b.Should().Throw<ArgumentException>();
      }


      [Test]
      public async Task transactions
      (
         [Values(null, "", "  ")]string accountId)
      {
         Func<Task<PagedResponse<Transaction>>> a = async () => await client.Transactions.ListTransactionsAsync(accountId);
         Func<Task<Response<Transaction>>> b = async () => await client.Transactions.SendMoneyAsync(accountId, null);
         Func<Task<Response<Transaction>>> c = async () => await client.Transactions.TransferMoneyAsync(accountId, null);
         Func<Task<Response<Transaction>>> d = async () => await client.Transactions.RequestMoneyAsync(accountId, null);

         a.Should().Throw<ArgumentException>();
         b.Should().Throw<ArgumentException>();
         c.Should().Throw<ArgumentException>();
         d.Should().Throw<ArgumentException>();
      }

      [Test]
      public async Task transactions2
      (
         [Values(null, "", "  ", "fff")]string accountId,
         [Values(null, "", "  ")]string txId)
      {
         Func<Task<Response<Transaction>>> a = async () => await client.Transactions.GetTransactionAsync(accountId, txId);
         Func<Task<IFlurlResponse>> b = async () => await client.Transactions.CompleteRequestMoneyAsync(accountId, txId);
         Func<Task<IFlurlResponse>> c = async () => await client.Transactions.ResendRequestMoneyAsync(accountId, txId);
         Func<Task<IFlurlResponse>> d = async () => await client.Transactions.CancelRequestMoneyAsync(accountId, txId);

         a.Should().Throw<ArgumentException>();
         b.Should().Throw<ArgumentException>();
         c.Should().Throw<ArgumentException>();
         d.Should().Throw<ArgumentException>();
      }


      [Test]
      public async Task notifications
      (
         [Values(null, "", "  ")]string notificationId)
      {
         Func<Task<Response<Notification>>> a = async () => await client.Notifications.GetNotificationAsync(notificationId);

         a.Should().Throw<ArgumentException>();
      }


      [Test]
      public async Task deposits
      (
         [Values(null, "", "  ")]string accountId)
      {
         Func<Task<PagedResponse<Deposit>>> a = async () => await client.Deposits.ListDepositsAsync(accountId);
         Func<Task<Response<Deposit>>> b = async () => await client.Deposits.DepositFundsAsync(accountId, null);

         a.Should().Throw<ArgumentException>();
         b.Should().Throw<ArgumentException>();
      }

      [Test]
      public async Task deposits2
      (
         [Values(null, "", "  ", "fff")]string accountId,
         [Values(null, "", "  ")]string depositId)
      {
         Func<Task<Response<Deposit>>> a = async () => await client.Deposits.GetDepositAsync(accountId, depositId);
         Func<Task<Response<Deposit>>> b = async () => await client.Deposits.CommitDepositAsync(accountId, depositId);

         a.Should().Throw<ArgumentException>();
         b.Should().Throw<ArgumentException>();
      }


      [Test]
      public async Task sells
      (
         [Values(null, "", "  ")]string accountId)
      {
         Func<Task<PagedResponse<Sell>>> a = async () => await client.Sells.ListSellsAsync(accountId);
         Func<Task<Response<Sell>>> b = async () => await client.Sells.PlaceSellOrderAsync(accountId, null);

         a.Should().Throw<ArgumentException>();
         b.Should().Throw<ArgumentException>();
      }

      [Test]
      public async Task sells2
      (
         [Values(null, "", "  ", "fff")]string accountId,
         [Values(null, "", "  ")]string sellId)
      {
         Func<Task<Response<Sell>>> a = async () => await client.Sells.GetSellAsync(accountId, sellId);
         Func<Task<Response<Sell>>> b = async () => await client.Sells.CommitSellAsync(accountId, sellId);

         a.Should().Throw<ArgumentException>();
         b.Should().Throw<ArgumentException>();
      }

      [Test]
      public async Task paymentmethod
      (
         [Values(null, "", "  ")]string paymentMethodId)
      {
         Func<Task<Response<PaymentMethod>>> a = async () => await client.PaymentMethods.GetPaymentMethodAsync(paymentMethodId);

         a.Should().Throw<ArgumentException>();
      }

      [Test]
      public async Task user
      (
         [Values(null, "", "  ")]string userId)
      {
         Func<Task<Response<User>>> a = async () => await client.Users.GetUserAsync(userId);

         a.Should().Throw<ArgumentException>();
      }

      [Test]
      public async Task data
      (
         [Values(null, "", "  ")]string currencyId)
      {
         Func<Task<Response<Money>>> a = async () => await client.Data.GetBuyPriceAsync(currencyId);
         Func<Task<Response<Money>>> b = async () => await client.Data.GetSellPriceAsync(currencyId);
         Func<Task<Response<Money>>> c = async () => await client.Data.GetSpotPriceAsync(currencyId);

         a.Should().Throw<ArgumentException>();
         b.Should().Throw<ArgumentException>();
         c.Should().Throw<ArgumentException>();
      }

      [Test]
      public async Task accounts
      (
         [Values(null, "", "  ")]string accountId)
      {
         Func<Task<Response<Account>>> a = async () => await client.Accounts.GetAccountAsync(accountId);
         Func<Task<Response<Account>>> b = async () => await client.Accounts.SetAccountAsPrimaryAsync(accountId);
         Func<Task<Response<Account>>> c = async () => await client.Accounts.UpdateAccountAsync(accountId, null);
         Func<Task<IFlurlResponse>> d = async () => await client.Accounts.DeleteAccountAsync(accountId);

         a.Should().Throw<ArgumentException>();
         b.Should().Throw<ArgumentException>();
         c.Should().Throw<ArgumentException>();
         d.Should().Throw<ArgumentException>();
      }

      [Test]
      public async Task buys
      (
         [Values(null, "", "  ")]string accountId)
      {
         var create = new PlaceBuy
            {
               Amount = 0.1m,
               Currency = "BTC",
               PaymentMethod = "B28EB04F-BD70-4308-90A1-96065283A001"
            };

         Func<Task<Response<Buy>>> a = async () => await client.Buys.PlaceBuyOrderAsync(accountId, create);

         a.Should().Throw<ArgumentException>();
      }

      [Test]
      public async Task buys2
      (
         [Values(null, "", "  ", "fff")] string accountId,
         [Values(null, "", "  ")] string buyId)
      {
         Func<Task<Response<Buy>>> a = async () => await client.Buys.CommitBuyAsync(accountId, buyId);
         Func<Task<Response<Buy>>> b = async () => await client.Buys.GetBuyAsync(accountId, buyId);

         a.Should().Throw<ArgumentException>();
         b.Should().Throw<ArgumentException>();
      }
   }
}
