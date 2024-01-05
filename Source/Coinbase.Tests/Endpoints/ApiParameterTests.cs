using System;
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
      public override void BeforeEachTest()
      {
         base.BeforeEachTest();
         client = new CoinbaseClient(new OAuthConfig {AccessToken = OAuthServerTest.OauthKey});
      }

      [Test]
      public async Task withdraw
      (
         [Values(null, "", "  ")]string accountId)
      {
         Func<Task<PagedResponse<Withdrawal>>> a = async () => await client.Withdrawals.ListWithdrawalsAsync(accountId);
         Func<Task<Response<Withdrawal>>> b = async () => await client.Withdrawals.WithdrawalFundsAsync(accountId, null);

         await a.Should().ThrowAsync<ArgumentException>();
         await b.Should().ThrowAsync<ArgumentException>();
      }

      [Test]
      public async Task withdraw2
      (
         [Values(null, "", "  ", "fff")]string accountId,
         [Values(null, "", "  ")]string withdrawlId)
      {
         Func<Task<Response<Withdrawal>>> a = async () => await client.Withdrawals.GetWithdrawalAsync(accountId, withdrawlId);
         Func<Task<Response<Withdrawal>>> b = async () => await client.Withdrawals.CommitWithdrawalAsync(accountId, withdrawlId);

         await a.Should().ThrowAsync<ArgumentException>();
         await b.Should().ThrowAsync<ArgumentException>();
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

         await a.Should().ThrowAsync<ArgumentException>();
            await b.Should().ThrowAsync<ArgumentException>();
         await c.Should().ThrowAsync<ArgumentException>();
         await d.Should().ThrowAsync<ArgumentException>();
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

         await a.Should().ThrowAsync<ArgumentException>();
         await b.Should().ThrowAsync<ArgumentException>();
         await c.Should().ThrowAsync<ArgumentException>();
         await d.Should().ThrowAsync<ArgumentException>();
      }

      [Test]
      public async Task notifications
      (
         [Values(null, "", "  ")]string notificationId)
      {
         Func<Task<Response<Notification>>> a = async () => await client.Notifications.GetNotificationAsync(notificationId);

         await a.Should().ThrowAsync<ArgumentException>();
      }

      [Test]
      public async Task deposits
      (
         [Values(null, "", "  ")]string accountId)
      {
         Func<Task<PagedResponse<Deposit>>> a = async () => await client.Deposits.ListDepositsAsync(accountId);
         Func<Task<Response<Deposit>>> b = async () => await client.Deposits.DepositFundsAsync(accountId, null);

         await a.Should().ThrowAsync<ArgumentException>();
         await b.Should().ThrowAsync<ArgumentException>();
      }

      [Test]
      public async Task deposits2
      (
         [Values(null, "", "  ", "fff")]string accountId,
         [Values(null, "", "  ")]string depositId)
      {
         Func<Task<Response<Deposit>>> a = async () => await client.Deposits.GetDepositAsync(accountId, depositId);
         Func<Task<Response<Deposit>>> b = async () => await client.Deposits.CommitDepositAsync(accountId, depositId);

         await a.Should().ThrowAsync<ArgumentException>();
         await b.Should().ThrowAsync<ArgumentException>();
      }

      [Test]
      public async Task paymentmethod
      (
         [Values(null, "", "  ")]string paymentMethodId)
      {
         Func<Task<Response<PaymentMethod>>> a = async () => await client.PaymentMethods.GetPaymentMethodAsync(paymentMethodId);

         await a.Should().ThrowAsync<ArgumentException>();
      }

      [Test]
      public async Task data
      (
         [Values(null, "", "  ")]string currencyId)
      {
         Func<Task<Response<Money>>> a = async () => await client.Data.GetBuyPriceAsync(currencyId);
         Func<Task<Response<Money>>> b = async () => await client.Data.GetSellPriceAsync(currencyId);
         Func<Task<Response<Money>>> c = async () => await client.Data.GetSpotPriceAsync(currencyId);

         await a.Should().ThrowAsync<ArgumentException>();
         await b.Should().ThrowAsync<ArgumentException>();
         await c.Should().ThrowAsync<ArgumentException>();
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

         await a.Should().ThrowAsync<ArgumentException>();
         await b.Should().ThrowAsync<ArgumentException>();
         await c.Should().ThrowAsync<ArgumentException>();
         await d.Should().ThrowAsync<ArgumentException>();
      }
   }
}
