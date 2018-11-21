using System;
using System.Collections.Generic;
using Coinbase.Models;
using Newtonsoft.Json.Linq;
using Z.ExtensionMethods;


namespace Coinbase.Tests
{
   public class Examples
   {
      public const string PaginationJson = @"
   {
    ""ending_before"": null,
    ""starting_after"": null,
    ""limit"": 25,
    ""order"": ""desc"",
    ""previous_uri"": null,
    ""next_uri"": ""/v2/accounts?&limit=25&starting_after=5d5aed5f-b7c0-5585-a3dd-a7ed9ef0e414""
  }";

      public static Pagination PaginationModel => new Pagination
         {
            Limit = 25,
            Order = SortOrder.Desc,
            NextUri = "/v2/accounts?&limit=25&starting_after=5d5aed5f-b7c0-5585-a3dd-a7ed9ef0e414"
         };

      public const string User = @"{
    ""id"": ""9da7a204-544e-5fd1-9a12-61176c5d4cd8"",
    ""name"": ""User One"",
    ""username"": ""user1"",
    ""profile_location"": null,
    ""profile_bio"": null,
    ""profile_url"": ""https://coinbase.com/user1"",
    ""avatar_url"": ""https://images.coinbase.com/avatar?h=vR%2FY8igBoPwuwGren5JMwvDNGpURAY%2F0nRIOgH%2FY2Qh%2BQ6nomR3qusA%2Bh6o2%0Af9rH&s=128"",
    ""resource"": ""user"",
    ""resource_path"": ""/v2/user/9da7a204-544e-5fd1-9a12-61176c5d4cd8""
  }";

      public static User UserModel => new User
         {
            Id = "9da7a204-544e-5fd1-9a12-61176c5d4cd8",
            Name = "User One",
            Username = "user1",
            ProfileUrl = "https://coinbase.com/user1",
            AvatarUrl = "https://images.coinbase.com/avatar?h=vR%2FY8igBoPwuwGren5JMwvDNGpURAY%2F0nRIOgH%2FY2Qh%2BQ6nomR3qusA%2Bh6o2%0Af9rH&s=128",
            Resource = "user",
            ResourcePath = "/v2/user/9da7a204-544e-5fd1-9a12-61176c5d4cd8"
         };

      public static string UserWithNameChange(string name)
         => User.Replace("User One", name);
      

      public const string Auth = @"{
    ""method"": ""oauth"",
    ""scopes"": [
        ""wallet:user:read"",
        ""wallet:user:email""
    ],
    ""oauth_meta"": {}
  }";

      public static Auth AuthModel => new Auth
      {
         Method = "oauth",
         Scopes = new[]
               {
                  "wallet:user:read",
                  "wallet:user:email"
               },
         OAuthMeta =  new Dictionary<string, JToken>()
      };

      public const string Account1 = @"{
      ""id"": ""58542935-67b5-56e1-a3f9-42686e07fa40"",
      ""name"": ""My Vault"",
      ""primary"": false,
      ""type"": ""vault"",
      ""currency"": {""code"":""BTC""},
      ""balance"": {
        ""amount"": ""4.00000000"",
        ""currency"": ""BTC""
      },
      ""created_at"": ""2015-01-31T20:49:02Z"",
      ""updated_at"": ""2015-01-31T20:49:02Z"",
      ""resource"": ""account"",
      ""resource_path"": ""/v2/accounts/58542935-67b5-56e1-a3f9-42686e07fa40"",
      ""ready"": true
    }";

      public const string Account2 = @"{
      ""id"": ""2bbf394c-193b-5b2a-9155-3b4732659ede"",
      ""name"": ""My Wallet"",
      ""primary"": true,
      ""type"": ""wallet"",
      ""currency"": {""code"":""BTC""},
      ""balance"": {
        ""amount"": ""39.59000000"",
        ""currency"": ""BTC""
      },
      ""created_at"": ""2015-01-31T20:49:02Z"",
      ""updated_at"": ""2015-01-31T20:49:02Z"",
      ""resource"": ""account"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede""
    }";

      public const string Account3 = @"{
    ""id"": ""82de7fcd-db72-5085-8ceb-bee19303080b"",
    ""name"": ""New hot wallet"",
    ""primary"": true,
    ""type"": ""wallet"",
    ""currency"": {""code"":""BTC""},
    ""balance"": {
      ""amount"": ""0.00000000"",
      ""currency"": ""BTC""
    },
    ""created_at"": ""2015-03-31T15:21:58-07:00"",
    ""updated_at"": ""2015-03-31T15:21:58-07:00"",
    ""resource"": ""account"",
    ""resource_path"": ""/v2/accounts/82de7fcd-db72-5085-8ceb-bee19303080b""
  }";

      public static string Account3WithNameChange(string name)
         => Account3.Replace("New hot wallet", name);

      public static Account Account1Model => new Account
      {
         Id = "58542935-67b5-56e1-a3f9-42686e07fa40",
         Name = "My Vault",
         Primary =  false,
         Type = "vault",
         Currency = new AccountCurrency
            {
               Code = "BTC"
            },
         Balance = new Money { Amount = 4, Currency = "BTC"},
         CreatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
         UpdatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
         Resource = "account",
         ResourcePath = "/v2/accounts/58542935-67b5-56e1-a3f9-42686e07fa40",
         ExtraJson = new Dictionary<string, JToken> { { "ready", true } }
      };

      public static Account Account2Model => new Account
         {
            Id = "2bbf394c-193b-5b2a-9155-3b4732659ede",
            Name = "My Wallet",
            Primary = true,
            Type = "wallet",
            Currency = new AccountCurrency
               {
                  Code = "BTC"
               },
            Balance = new Money { Amount = 39.59m, Currency = "BTC" },
            CreatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
            UpdatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
            Resource = "account",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede"
         };

      public static Account Account3Model => new Account
         {
            Id = "82de7fcd-db72-5085-8ceb-bee19303080b",
            Name = "New hot wallet",
            Primary = true,
            Type = "wallet",
            Currency = new AccountCurrency
               {
                  Code = "BTC"
               },
            Balance = new Money { Amount = 0, Currency = "BTC" },
            CreatedAt = DateTimeOffset.Parse("2015-03-31T15:21:58-07:00"),
            UpdatedAt = DateTimeOffset.Parse("2015-03-31T15:21:58-07:00"),
            Resource = "account",
            ResourcePath = "/v2/accounts/82de7fcd-db72-5085-8ceb-bee19303080b"
      };


      public const string Address1 = @"{
      ""id"": ""dd3183eb-af1d-5f5d-a90d-cbff946435ff"",
      ""address"": ""mswUGcPHp1YnkLCgF1TtoryqSc5E9Q8xFa"",
      ""name"": null,
      ""created_at"": ""2015-01-31T20:49:02Z"",
      ""updated_at"": ""2015-03-31T17:25:29-07:00"",
      ""network"": ""bitcoin"",
      ""resource"": ""address"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/addresses/dd3183eb-af1d-5f5d-a90d-cbff946435ff""
    }";

      public static string Address1WithName(string name)
      {
         return Address1.Replace(@"""name"": null,", $@"""name"": ""{name}"",");
      }

      public const string Address2 = @"{
      ""id"": ""ac5c5f15-0b1d-54f5-8912-fecbf66c2a64"",
      ""address"": ""mgSvu1z1amUFAPkB4cUg8ujaDxKAfZBt5Q"",
      ""name"": null,
      ""created_at"": ""2015-03-31T17:23:52-07:00"",
      ""updated_at"": ""2015-01-31T20:49:02Z"",
      ""network"": ""bitcoin"",
      ""resource"": ""address"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/addresses/ac5c5f15-0b1d-54f5-8912-fecbf66c2a64""
    }";

      public static AddressEntity Address1Model => new AddressEntity
      {
         Id = "dd3183eb-af1d-5f5d-a90d-cbff946435ff",
         Address = "mswUGcPHp1YnkLCgF1TtoryqSc5E9Q8xFa",
         CreatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
         UpdatedAt = DateTimeOffset.Parse("2015-03-31T17:25:29-07:00"),
         Network = "bitcoin",
         Resource = "address",
         ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/addresses/dd3183eb-af1d-5f5d-a90d-cbff946435ff"
      };
      public static AddressEntity Address2Model => new AddressEntity
      {
         Id = "ac5c5f15-0b1d-54f5-8912-fecbf66c2a64",
         Address = "mgSvu1z1amUFAPkB4cUg8ujaDxKAfZBt5Q",
         CreatedAt = DateTimeOffset.Parse("2015-03-31T17:23:52-07:00"),
         UpdatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
         Network = "bitcoin",
         Resource = "address",
         ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/addresses/ac5c5f15-0b1d-54f5-8912-fecbf66c2a64"
      };

      public const string Transaction1 = @"{
      ""id"": ""57ffb4ae-0c59-5430-bcd3-3f98f797a66c"",
      ""type"": ""send"",
      ""status"": ""completed"",
      ""amount"": {
        ""amount"": ""0.00100000"",
        ""currency"": ""BTC""
      },
      ""native_amount"": {
        ""amount"": ""0.01"",
        ""currency"": ""USD""
      },
      ""description"": null,
      ""created_at"": ""2015-03-11T13:13:35-07:00"",
      ""updated_at"": ""2015-03-26T15:55:43-07:00"",
      ""resource"": ""transaction"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/57ffb4ae-0c59-5430-bcd3-3f98f797a66c"",
      ""network"": {
        ""status"": ""off_blockchain"",
        ""name"": ""bitcoin""
      },
      ""from"": {
        ""id"": ""a6b4c2df-a62c-5d68-822a-dd4e2102e703"",
        ""resource"": ""user""
      },
      ""instant_exchange"": false
    }";

      public static Transaction Transaction1Model => new Transaction
         {
            Id = "57ffb4ae-0c59-5430-bcd3-3f98f797a66c",
            Type = "send",
            Status = "completed",
            Amount = new Money { Amount = 0.00100000m, Currency = "BTC"},
            NativeAmount = new Money { Amount = 0.01m, Currency = "USD"},
            CreatedAt = DateTimeOffset.Parse("2015-03-11T13:13:35-07:00"),
            UpdatedAt = DateTimeOffset.Parse("2015-03-26T15:55:43-07:00"),
            Resource = "transaction",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/57ffb4ae-0c59-5430-bcd3-3f98f797a66c",
            Network = new Network { Status = "off_blockchain", Name = "bitcoin"},
            From = new From { Id = "a6b4c2df-a62c-5d68-822a-dd4e2102e703", Resource = "user"},
            InstantExchange = false
         };


      public const string Transaction2 = @"{
      ""id"": ""4117f7d6-5694-5b36-bc8f-847509850ea4"",
      ""type"": ""buy"",
      ""status"": ""pending"",
      ""amount"": {
        ""amount"": ""486.34313725"",
        ""currency"": ""BTC""
      },
      ""native_amount"": {
        ""amount"": ""4863.43"",
        ""currency"": ""USD""
      },
      ""description"": null,
      ""created_at"": ""2015-03-26T23:44:08-07:00"",
      ""updated_at"": ""2015-03-26T23:44:08-07:00"",
      ""resource"": ""transaction"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/4117f7d6-5694-5b36-bc8f-847509850ea4"",
      ""buy"": {
        ""id"": ""9e14d574-30fa-5d85-b02c-6be0d851d61d"",
        ""resource"": ""buy"",
        ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/buys/9e14d574-30fa-5d85-b02c-6be0d851d61d""
      },
      ""details"": {
        ""title"": ""Bought bitcoin"",
        ""subtitle"": ""using Capital One Bank""
      }
    }";

      public static Transaction Transaction2Model => new Transaction
         {
            Id = "4117f7d6-5694-5b36-bc8f-847509850ea4",
            Type = "buy",
            Status = "pending",
            Amount = new Money {Amount = 486.34313725m, Currency = "BTC"},
            NativeAmount = new Money {Amount = 4863.43m, Currency = "USD"},
            Description = null,
            CreatedAt = DateTimeOffset.Parse("2015-03-26T23:44:08-07:00"),
            UpdatedAt = DateTimeOffset.Parse("2015-03-26T23:44:08-07:00"),
            Resource = "transaction",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/4117f7d6-5694-5b36-bc8f-847509850ea4",
            Network = null,
            Buy = new Entity
               {
                  Id = "9e14d574-30fa-5d85-b02c-6be0d851d61d",
                  Resource = "buy",
                  ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/buys/9e14d574-30fa-5d85-b02c-6be0d851d61d"
               },
            Details = new Dictionary<string, JToken>
               {
                  {"title", "Bought bitcoin"},
                  {"subtitle", "using Capital One Bank"}
               }
         };

      public const string Transaction3 = @"{
      ""id"": ""005e55d1-f23a-5d1e-80a4-72943682c055"",
      ""type"": ""request"",
      ""status"": ""pending"",
      ""amount"": {
        ""amount"": ""0.10000000"",
        ""currency"": ""BTC""
      },
      ""native_amount"": {
        ""amount"": ""1.00"",
        ""currency"": ""USD""
      },
      ""description"": """",
      ""created_at"": ""2015-03-24T18:32:35-07:00"",
      ""updated_at"": ""2015-01-31T20:49:02Z"",
      ""resource"": ""transaction"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/005e55d1-f23a-5d1e-80a4-72943682c055"",
      ""to"": {
        ""resource"": ""email"",
        ""email"": ""rb@coinbase.com""
      },
      ""details"": {
        ""title"": ""Requested bitcoin"",
        ""subtitle"": ""from rb@coinbase.com""
      }
    }";

      public static Transaction Transaction3Model => new Transaction
         {
            Id = "005e55d1-f23a-5d1e-80a4-72943682c055",
            Type = "request",
            Status = "pending",
            Amount = new Money {Amount = 0.10000000m, Currency = "BTC"},
            NativeAmount = new Money {Amount = 1.00m, Currency = "USD"},
            Description = "",
            CreatedAt = DateTimeOffset.Parse("2015-03-24T18:32:35-07:00"),
            UpdatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
            Resource = "transaction",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/005e55d1-f23a-5d1e-80a4-72943682c055",
            Network = null,
            To = new To
               {
                  Resource = "email",
                  ExtraJson = new Dictionary<string, JToken>
                     {
                        {"email", "rb@coinbase.com"}
                     }
               },
            Details = new Dictionary<string, JToken>
               {
                  {"title", "Requested bitcoin"},
                  {"subtitle", "from rb@coinbase.com"}
               }
         };

      public const string Transaction4 = @"{
      ""id"": ""ff01bbc6-c4ad-59e1-9601-e87b5b709458"",
      ""type"": ""transfer"",
      ""status"": ""completed"",
      ""amount"": {
        ""amount"": ""-5.00000000"",
        ""currency"": ""BTC""
      },
      ""native_amount"": {
        ""amount"": ""-50.00"",
        ""currency"": ""USD""
      },
      ""description"": """",
      ""created_at"": ""2015-03-12T15:51:38-07:00"",
      ""updated_at"": ""2015-01-31T20:49:02Z"",
      ""resource"": ""transaction"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/ff01bbc6-c4ad-59e1-9601-e87b5b709458"",
      ""to"": {
        ""id"": ""58542935-67b5-56e1-a3f9-42686e07fa40"",
        ""resource"": ""account"",
        ""resource_path"": ""/v2/accounts/58542935-67b5-56e1-a3f9-42686e07fa40""
      },
      ""details"": {
        ""title"": ""Transfered bitcoin"",
        ""subtitle"": ""to Secondary Account""
      }
    }";

      public static Transaction Transaction4Model => new Transaction
         {
            Id = "ff01bbc6-c4ad-59e1-9601-e87b5b709458",
            Type = "transfer",
            Status = "completed",
            Amount = new Money { Amount = -5.00000000m, Currency = "BTC" },
            NativeAmount = new Money { Amount = -50.00m, Currency = "USD" },
            Description = "",
            CreatedAt = DateTimeOffset.Parse("2015-03-12T15:51:38-07:00"),
            UpdatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
            Resource = "transaction",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/ff01bbc6-c4ad-59e1-9601-e87b5b709458",
            To = new To
               {
                  Id = "58542935-67b5-56e1-a3f9-42686e07fa40",
                  Resource = "account",
                  ResourcePath = "/v2/accounts/58542935-67b5-56e1-a3f9-42686e07fa40"
            },
            Details = new Dictionary<string, JToken>
               {
                  {"title", "Transfered bitcoin"},
                  {"subtitle", "to Secondary Account"}
               }
         };

      public const string Transaction5 = @"{
      ""id"": ""57ffb4ae-0c59-5430-bcd3-3f98f797a66c"",
      ""type"": ""send"",
      ""status"": ""completed"",
      ""amount"": {
        ""amount"": ""-0.00100000"",
        ""currency"": ""BTC""
      },
      ""native_amount"": {
        ""amount"": ""-0.01"",
        ""currency"": ""USD""
      },
      ""description"": null,
      ""created_at"": ""2015-03-11T13:13:35-07:00"",
      ""updated_at"": ""2015-03-26T15:55:43-07:00"",
      ""resource"": ""transaction"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/57ffb4ae-0c59-5430-bcd3-3f98f797a66c"",
      ""network"": {
        ""status"": ""off_blockchain"",
        ""name"": ""bitcoin""
      },
      ""to"": {
        ""id"": ""a6b4c2df-a62c-5d68-822a-dd4e2102e703"",
        ""resource"": ""user"",
        ""resource_path"": ""/v2/users/a6b4c2df-a62c-5d68-822a-dd4e2102e703""
      },
      ""details"": {
        ""title"": ""Send bitcoin"",
        ""subtitle"": ""to User 2""
      }
    }";


      public static Transaction Transaction5Model => new Transaction
         {
            Id = "57ffb4ae-0c59-5430-bcd3-3f98f797a66c",
            Type = "send",
            Status = "completed",
            Amount = new Money { Amount = -0.00100000m, Currency = "BTC" },
            NativeAmount = new Money { Amount = -0.01m, Currency = "USD" },
            Description = null,
            CreatedAt = DateTimeOffset.Parse("2015-03-11T13:13:35-07:00"),
            UpdatedAt = DateTimeOffset.Parse("2015-03-26T15:55:43-07:00"),
            Resource = "transaction",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/57ffb4ae-0c59-5430-bcd3-3f98f797a66c",
            Network = new Network
               {
                  Status = "off_blockchain",
                  Name = "bitcoin"
               },
            To = new To
               {
                  Id = "a6b4c2df-a62c-5d68-822a-dd4e2102e703",
                  Resource = "user",
                  ResourcePath = "/v2/users/a6b4c2df-a62c-5d68-822a-dd4e2102e703"
            },
            Details = new Dictionary<string, JToken>
               {
                  {"title", "Send bitcoin"},
                  {"subtitle", "to User 2"}
               }
         };

      public const string Transaction6 = @"{
    ""id"": ""3c04e35e-8e5a-5ff1-9155-00675db4ac02"",
    ""type"": ""send"",
    ""status"": ""pending"",
    ""amount"": {
      ""amount"": ""-0.10000000"",
      ""currency"": ""BTC""
    },
    ""native_amount"": {
      ""amount"": ""-1.00"",
      ""currency"": ""USD""
    },
    ""description"": null,
    ""created_at"": ""2015-01-31T20:49:02Z"",
    ""updated_at"": ""2015-03-31T17:25:29-07:00"",
    ""resource"": ""transaction"",
    ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/3c04e35e-8e5a-5ff1-9155-00675db4ac02"",
    ""network"": {
      ""status"": ""unconfirmed"",
      ""hash"": ""463397c87beddd9a61ade61359a13adc9efea26062191fe07147037bce7f33ed"",
      ""name"": ""bitcoin""
    },
    ""to"": {
      ""resource"": ""bitcoin_address"",
      ""address"": ""1AUJ8z5RuHRTqD1eikyfUUetzGmdWLGkpT""
    },
    ""details"": {
      ""title"": ""Send bitcoin"",
      ""subtitle"": ""to User 2""
    }
  }";

      public static Transaction Transaction6Model => new Transaction
         {
            Id = "3c04e35e-8e5a-5ff1-9155-00675db4ac02",
            Type = "send",
            Status = "pending",
            Amount = new Money { Amount = -0.10000000m, Currency = "BTC" },
            NativeAmount = new Money { Amount = -1.00m, Currency = "USD" },
            Description = null,
            CreatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
            UpdatedAt = DateTimeOffset.Parse("2015-03-31T17:25:29-07:00"),
            Resource = "transaction",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/3c04e35e-8e5a-5ff1-9155-00675db4ac02",
            Network = new Network
               {
                  Status = "unconfirmed",
                  Hash = "463397c87beddd9a61ade61359a13adc9efea26062191fe07147037bce7f33ed",
                  Name = "bitcoin"
            },
            To = new To
               {
                  Resource = "bitcoin_address",
                  Address = "1AUJ8z5RuHRTqD1eikyfUUetzGmdWLGkpT"
               },
            Details = new Dictionary<string, JToken>
               {
                  {"title", "Send bitcoin"},
                  {"subtitle", "to User 2"}
               }
         };

      public const string Transaction7 = @"{
    ""id"": ""2e9f48cd-0b05-5f7c-9056-17a8acb408ad"",
    ""type"": ""request"",
    ""status"": ""pending"",
    ""amount"": {
      ""amount"": ""1.00000000"",
      ""currency"": ""BTC""
    },
    ""native_amount"": {
      ""amount"": ""10.00"",
      ""currency"": ""USD""
    },
    ""description"": null,
    ""created_at"": ""2015-04-01T10:37:11-07:00"",
    ""updated_at"": ""2015-04-01T10:37:11-07:00"",
    ""resource"": ""transaction"",
    ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/2e9f48cd-0b05-5f7c-9056-17a8acb408ad"",
    ""to"": {
      ""resource"": ""email"",
      ""email"": ""email@example.com""
    },
    ""details"": {
      ""title"": ""Requested bitcoin"",
      ""subtitle"": ""from email@example.com""
    }
  }";

      public static Transaction Transaction7Model => new Transaction
      {
         Id = "2e9f48cd-0b05-5f7c-9056-17a8acb408ad",
         Type = "request",
         Status = "pending",
         Amount = new Money { Amount = 1.00000000m, Currency = "BTC" },
         NativeAmount = new Money { Amount = 10.00m, Currency = "USD" },
         Description = null,
         CreatedAt = DateTimeOffset.Parse("2015-04-01T10:37:11-07:00"),
         UpdatedAt = DateTimeOffset.Parse("2015-04-01T10:37:11-07:00"),
         Resource = "transaction",
         ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/2e9f48cd-0b05-5f7c-9056-17a8acb408ad",
         To = new To
         {
            Resource = "email",
            ExtraJson = new Dictionary<string, JToken>
                     {
                        {"email", "email@example.com"}
                     }
         },
         Details = new Dictionary<string, JToken>
               {
                  {"title", "Requested bitcoin"},
                  {"subtitle", "from email@example.com"}
               }
      };


      public const string Buy1 = @"{
      ""id"": ""9e14d574-30fa-5d85-b02c-6be0d851d61d"",
      ""status"": ""created"",
      ""payment_method"": {
        ""id"": ""83562370-3e5c-51db-87da-752af5ab9559"",
        ""resource"": ""payment_method"",
        ""resource_path"": ""/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559""
      },
      ""transaction"": {
        ""id"": ""4117f7d6-5694-5b36-bc8f-847509850ea4"",
        ""resource"": ""transaction"",
        ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a""
      },
      ""amount"": {
        ""amount"": ""10.00000000"",
        ""currency"": ""BTC""
      },
      ""total"": {
        ""amount"": ""102.01"",
        ""currency"": ""USD""
      },
      ""subtotal"": {
        ""amount"": ""101.00"",
        ""currency"": ""USD""
      },
      ""created_at"": ""2015-03-26T23:43:59-07:00"",
      ""updated_at"": ""2015-03-26T23:44:09-07:00"",
      ""resource"": ""buy"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/buys/9e14d574-30fa-5d85-b02c-6be0d851d61d"",
      ""committed"": true,
      ""instant"": false,
      ""fee"": {
        ""amount"": ""1.01"",
        ""currency"": ""USD""
      },
      ""payout_at"": ""2015-04-01T23:43:59-07:00""
    }";

      public static Buy Buy1Model => new Buy
         {
         Id = "9e14d574-30fa-5d85-b02c-6be0d851d61d",
         Status = "created",
         PaymentMethod = new Entity
         {
            Id = "83562370-3e5c-51db-87da-752af5ab9559",
            Resource = "payment_method",
            ResourcePath = "/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559"
         },
         Transaction = new Entity
         {
            Id = "4117f7d6-5694-5b36-bc8f-847509850ea4",
            Resource = "transaction",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a"
         },
         Amount = new Money
         {
            Amount = 10.00000000m,
            Currency = "BTC"
         },
         Total = new Money
         {
            Amount = 102.01m,
            Currency = "USD"
         },
         Subtotal = new Money
         {
            Amount = 101.00m,
            Currency = "USD"
         },
         CreatedAt = DateTimeOffset.Parse("2015-03-26T23:43:59-07:00"),
         UpdatedAt = DateTimeOffset.Parse("2015-03-26T23:44:09-07:00"),
         Resource = "buy",
         ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/buys/9e14d574-30fa-5d85-b02c-6be0d851d61d",
         Committed = true,
         Instant = false,
         Fee = new Money
            {
               Amount = 1.01m,
               Currency = "USD"
            },
         PayoutAt = DateTimeOffset.Parse("2015-04-01T23:43:59-07:00")
      };

      public const string Buy2 = @"{
    ""id"": ""a333743d-184a-5b5b-abe8-11612fc44ab5"",
    ""status"": ""created"",
    ""payment_method"": {
      ""id"": ""83562370-3e5c-51db-87da-752af5ab9559"",
      ""resource"": ""payment_method"",
      ""resource_path"": ""/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559""
    },
    ""transaction"": {
      ""id"": ""763d1401-fd17-5a18-852a-9cca5ac2f9c0"",
      ""resource"": ""transaction"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a""
    },
    ""amount"": {
      ""amount"": ""10.00000000"",
      ""currency"": ""BTC""
    },
    ""total"": {
      ""amount"": ""102.01"",
      ""currency"": ""USD""
    },
    ""subtotal"": {
      ""amount"": ""101.00"",
      ""currency"": ""USD""
    },
    ""created_at"": ""2015-04-01T18:43:37-07:00"",
    ""updated_at"": ""2015-04-01T18:43:37-07:00"",
    ""resource"": ""buy"",
    ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/buys/a333743d-184a-5b5b-abe8-11612fc44ab5"",
    ""committed"": true,
    ""instant"": false,
    ""fee"": {
      ""amount"": ""1.01"",
      ""currency"": ""USD""
    },
    ""payout_at"": ""2015-04-07T18:43:37-07:00""
  }";

      public static Buy Buy2Model => new Buy
         {
            Id = "a333743d-184a-5b5b-abe8-11612fc44ab5",
            Status = "created",
            PaymentMethod = new Entity
               {
                  Id = "83562370-3e5c-51db-87da-752af5ab9559",
                  Resource = "payment_method",
                  ResourcePath = "/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559"
            },
            Transaction = new Entity
               {
                  Id = "763d1401-fd17-5a18-852a-9cca5ac2f9c0",
                  Resource = "transaction",
                  ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a"
            },
            Amount = new Money
               {
                  Amount = 10.00000000m,
                  Currency = "BTC"
               },
            Total = new Money
               {
                  Amount = 102.01m,
                  Currency = "USD"
               },
            Subtotal = new Money
               {
                  Amount = 101.00m,
                  Currency = "USD"
               },
            CreatedAt = DateTimeOffset.Parse("2015-04-01T18:43:37-07:00"),
            UpdatedAt = DateTimeOffset.Parse("2015-04-01T18:43:37-07:00"),
            Resource = "buy",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/buys/a333743d-184a-5b5b-abe8-11612fc44ab5",
            Committed = true,
            Instant = false,
            Fee = new Money
               {
                  Amount = 1.01m,
                  Currency = "USD"
               },
            PayoutAt = DateTimeOffset.Parse("2015-04-07T18:43:37-07:00")
         };

      public const string Sell1 = @"{
      ""id"": ""9e14d574-30fa-5d85-b02c-6be0d851d61d"",
      ""status"": ""created"",
      ""payment_method"": {
        ""id"": ""83562370-3e5c-51db-87da-752af5ab9559"",
        ""resource"": ""payment_method"",
        ""resource_path"": ""/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559""
      },
      ""transaction"": {
        ""id"": ""4117f7d6-5694-5b36-bc8f-847509850ea4"",
        ""resource"": ""transaction"",
        ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/4117f7d6-5694-5b36-bc8f-847509850ea4""
      },
      ""amount"": {
        ""amount"": ""10.00000000"",
        ""currency"": ""BTC""
      },
      ""total"": {
        ""amount"": ""98.01"",
        ""currency"": ""USD""
      },
      ""subtotal"": {
        ""amount"": ""99.00"",
        ""currency"": ""USD""
      },
      ""created_at"": ""2015-03-26T23:43:59-07:00"",
      ""updated_at"": ""2015-03-26T23:44:09-07:00"",
      ""resource"": ""sell"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/sells/9e14d574-30fa-5d85-b02c-6be0d851d61d"",
      ""committed"": true,
      ""instant"": false,
      ""fee"": {
        ""amount"": ""10.1"",
        ""currency"": ""USD""
      },
      ""payout_at"": ""2015-04-01T23:43:59-07:00""
    }";

      public static Sell Sell1Model => new Sell
         {
            Id = "9e14d574-30fa-5d85-b02c-6be0d851d61d",
            Status = "created",
            PaymentMethod = new Entity
               {
                  Id = "83562370-3e5c-51db-87da-752af5ab9559",
                  Resource = "payment_method",
                  ResourcePath = "/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559"
               },
            Transaction = new Entity
               {
                  Id = "4117f7d6-5694-5b36-bc8f-847509850ea4",
                  Resource = "transaction",
                  ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/4117f7d6-5694-5b36-bc8f-847509850ea4"
            },
            Amount = new Money
               {
                  Amount = 10.00000000m,
                  Currency = "BTC"
               },
            Total = new Money
               {
                  Amount = 98.01m,
                  Currency = "USD"
               },
            Subtotal = new Money
               {
                  Amount = 99.00m,
                  Currency = "USD"
               },
            CreatedAt = DateTimeOffset.Parse("2015-03-26T23:43:59-07:00"),
            UpdatedAt = DateTimeOffset.Parse("2015-03-26T23:44:09-07:00"),
            Resource = "sell",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/sells/9e14d574-30fa-5d85-b02c-6be0d851d61d",
            Committed = true,
            Instant = false,
            Fee = new Money
               {
                  Amount = 10.1m,
                  Currency = "USD"
               },
            PayoutAt = DateTimeOffset.Parse("2015-04-01T23:43:59-07:00")
      };



      public const string Sell2 = @"{
    ""id"": ""a333743d-184a-5b5b-abe8-11612fc44ab5"",
    ""status"": ""created"",
    ""payment_method"": {
      ""id"": ""83562370-3e5c-51db-87da-752af5ab9559"",
      ""resource"": ""payment_method"",
      ""resource_path"": ""/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559""
    },
    ""transaction"": {
      ""id"": ""763d1401-fd17-5a18-852a-9cca5ac2f9c0"",
      ""resource"": ""transaction"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/763d1401-fd17-5a18-852a-9cca5ac2f9c0""
    },
    ""amount"": {
      ""amount"": ""10.00000000"",
      ""currency"": ""BTC""
    },
    ""total"": {
      ""amount"": ""98.01"",
      ""currency"": ""USD""
    },
    ""subtotal"": {
      ""amount"": ""99.00"",
      ""currency"": ""USD""
    },
    ""created_at"": ""2015-04-01T18:43:37-07:00"",
    ""updated_at"": ""2015-04-01T18:43:37-07:00"",
    ""resource"": ""sell"",
    ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/sells/a333743d-184a-5b5b-abe8-11612fc44ab5"",
    ""committed"": true,
    ""instant"": false,
    ""fee"": {
      ""amount"": ""10.1"",
      ""currency"": ""USD""
    },
    ""payout_at"": ""2015-04-07T18:43:37-07:00""
  }";

      public static Sell Sell2Model => new Sell
      {
         Id = "a333743d-184a-5b5b-abe8-11612fc44ab5",
         Status = "created",
         PaymentMethod = new Entity
         {
            Id = "83562370-3e5c-51db-87da-752af5ab9559",
            Resource = "payment_method",
            ResourcePath = "/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559"
         },
         Transaction = new Entity
         {
            Id = "763d1401-fd17-5a18-852a-9cca5ac2f9c0",
            Resource = "transaction",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/763d1401-fd17-5a18-852a-9cca5ac2f9c0"
         },
         Amount = new Money
         {
            Amount = 10.00000000m,
            Currency = "BTC"
         },
         Total = new Money
            {
               Amount = 98.01m,
               Currency = "USD"
            },
         Subtotal = new Money
            {
               Amount = 99.00m,
               Currency = "USD"
            },
         CreatedAt = DateTimeOffset.Parse("2015-04-01T18:43:37-07:00"),
         UpdatedAt = DateTimeOffset.Parse("2015-04-01T18:43:37-07:00"),
         Resource = "sell",
         ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/sells/a333743d-184a-5b5b-abe8-11612fc44ab5",
         Committed = true,
         Instant = false,
         Fee = new Money
         {
            Amount = 10.1m,
            Currency = "USD"
         },
         PayoutAt = DateTimeOffset.Parse("2015-04-07T18:43:37-07:00")
      };




      public const string Deposit1 = @"{
      ""id"": ""67e0eaec-07d7-54c4-a72c-2e92826897df"",
      ""status"": ""completed"",
      ""payment_method"": {
        ""id"": ""83562370-3e5c-51db-87da-752af5ab9559"",
        ""resource"": ""payment_method"",
        ""resource_path"": ""/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559""
      },
      ""transaction"": {
        ""id"": ""441b9494-b3f0-5b98-b9b0-4d82c21c252a"",
        ""resource"": ""transaction"",
        ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a""
      },
      ""amount"": {
        ""amount"": ""10.00"",
        ""currency"": ""USD""
      },
      ""subtotal"": {
        ""amount"": ""10.00"",
        ""currency"": ""USD""
      },
      ""created_at"": ""2015-01-31T20:49:02Z"",
      ""updated_at"": ""2015-02-11T16:54:02-08:00"",
      ""resource"": ""deposit"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/deposits/67e0eaec-07d7-54c4-a72c-2e92826897df"",
      ""committed"": true,
      ""fee"": {
        ""amount"": ""0.00"",
        ""currency"": ""USD""
      },
      ""payout_at"": ""2015-02-18T16:54:00-08:00""
    }";

      public static Deposit Deposit1Model => new Deposit
         {
            Id = "67e0eaec-07d7-54c4-a72c-2e92826897df",
            Status = "completed",
            PaymentMethod = new Entity
               {
                  Id = "83562370-3e5c-51db-87da-752af5ab9559",
                  Resource = "payment_method",
                  ResourcePath = "/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559"
               },
            Transaction = new Entity
               {
                  Id = "441b9494-b3f0-5b98-b9b0-4d82c21c252a",
                  Resource = "transaction",
                  ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a"
            },
            Amount = new Money
               {
                  Amount = 10.00m,
                  Currency = "USD"
               },
            Subtotal = new Money
               {
                  Amount = 10.00m,
                  Currency = "USD"
               },
            CreatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
            UpdatedAt = DateTimeOffset.Parse("2015-02-11T16:54:02-08:00"),
            Resource = "deposit",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/deposits/67e0eaec-07d7-54c4-a72c-2e92826897df",
            Committed = true,
            Fee = new Money
               {
                  Amount = 0.00m,
                  Currency = "USD"
               },
            PayoutAt = DateTimeOffset.Parse("2015-02-18T16:54:00-08:00")
         };


      public const string Withdrawal1 = @"{
      ""id"": ""67e0eaec-07d7-54c4-a72c-2e92826897df"",
      ""status"": ""completed"",
      ""payment_method"": {
        ""id"": ""83562370-3e5c-51db-87da-752af5ab9559"",
        ""resource"": ""payment_method"",
        ""resource_path"": ""/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559""
      },
      ""transaction"": {
        ""id"": ""441b9494-b3f0-5b98-b9b0-4d82c21c252a"",
        ""resource"": ""transaction"",
        ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a""
      },
      ""amount"": {
        ""amount"": ""10.00"",
        ""currency"": ""USD""
      },
      ""subtotal"": {
        ""amount"": ""10.00"",
        ""currency"": ""USD""
      },
      ""created_at"": ""2015-01-31T20:49:02Z"",
      ""updated_at"": ""2015-02-11T16:54:02-08:00"",
      ""resource"": ""withdrawal"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/withdrawals/67e0eaec-07d7-54c4-a72c-2e92826897df"",
      ""committed"": true,
      ""fee"": {
        ""amount"": ""0.00"",
        ""currency"": ""USD""
      },
      ""payout_at"": ""2015-02-18T16:54:00-08:00""
    }";

      public static Withdrawal Withdrawal1Model => new Withdrawal
      {
         Id = "67e0eaec-07d7-54c4-a72c-2e92826897df",
         Status = "completed",
         PaymentMethod = new Entity
         {
            Id = "83562370-3e5c-51db-87da-752af5ab9559",
            Resource = "payment_method",
            ResourcePath = "/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559"
         },
         Transaction = new Entity
         {
            Id = "441b9494-b3f0-5b98-b9b0-4d82c21c252a",
            Resource = "transaction",
            ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a"
         },
         Amount = new Money
         {
            Amount = 10.00m,
            Currency = "USD"
         },
         Subtotal = new Money
         {
            Amount = 10.00m,
            Currency = "USD"
         },
         CreatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
         UpdatedAt = DateTimeOffset.Parse("2015-02-11T16:54:02-08:00"),
         Resource = "withdrawal",
         ResourcePath = "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/withdrawals/67e0eaec-07d7-54c4-a72c-2e92826897df",
         Committed = true,
         Fee = new Money
         {
            Amount = 0.00m,
            Currency = "USD"
         },
         PayoutAt = DateTimeOffset.Parse("2015-02-18T16:54:00-08:00")
      };


      public const string PayMethod1 = @"{
      ""id"": ""127b4d76-a1a0-5de7-8185-3657d7b526ec"",
      ""type"": ""fiat_account"",
      ""name"": ""USD Wallet"",
      ""currency"": ""USD"",
      ""primary_buy"": false,
      ""primary_sell"": false,
      ""allow_buy"": true,
      ""allow_sell"": true,
      ""allow_deposit"": true,
      ""allow_withdraw"": true,
      ""instant_buy"": true,
      ""instant_sell"": true,
      ""created_at"": ""2015-02-24T14:30:30-08:00"",
      ""updated_at"": ""2015-02-24T14:30:30-08:00"",
      ""resource"": ""payment_method"",
      ""resource_path"": ""/v2/payment-methods/127b4d76-a1a0-5de7-8185-3657d7b526ec"",
      ""fiat_account"": {
          ""id"": ""a077fff9-312b-559b-af98-146c33e27388"",
          ""resource"": ""account"",
          ""resource_path"": ""/v2/accounts/a077fff9-312b-559b-af98-146c33e27388""
      }
    }";

      public static PaymentMethod PayMethod1Model => new PaymentMethod
         {
            Id = "127b4d76-a1a0-5de7-8185-3657d7b526ec",
            Type = "fiat_account",
            Name = "USD Wallet",
            Currency = "USD",
            PrimaryBuy = false,
            PrimarySell = false,
            AllowBuy = true,
            AllowSell = true,
            AllowDeposit = true,
            AllowWithdraw = true,
            InstantBuy = true,
            InstantSell = true,
            CreatedAt = DateTimeOffset.Parse("2015-02-24T14:30:30-08:00"),
            UpdatedAt = DateTimeOffset.Parse("2015-02-24T14:30:30-08:00"),
            Resource = "payment_method",
            ResourcePath = "/v2/payment-methods/127b4d76-a1a0-5de7-8185-3657d7b526ec",
            ExtraJson = new Dictionary<string, JToken>
               {
                  {
                     "fiat_account", new JObject
                        {
                           {"id", "a077fff9-312b-559b-af98-146c33e27388"},
                           {"resource", "account"},
                           {"resource_path", "/v2/accounts/a077fff9-312b-559b-af98-146c33e27388"}
                        }
                  }
               }
         };

      public const string PayMethod2 = @"{
      ""id"": ""83562370-3e5c-51db-87da-752af5ab9559"",
      ""type"": ""ach_bank_account"",
      ""name"": ""International Bank *****1111"",
      ""currency"": ""USD"",
      ""primary_buy"": true,
      ""primary_sell"": true,
      ""allow_buy"": true,
      ""allow_sell"": true,
      ""allow_deposit"": true,
      ""allow_withdraw"": true,
      ""instant_buy"": false,
      ""instant_sell"": false,
      ""created_at"": ""2015-01-31T20:49:02Z"",
      ""updated_at"": ""2015-02-11T16:53:57-08:00"",
      ""resource"": ""payment_method"",
      ""resource_path"": ""/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559""
    }";

      public static PaymentMethod PayMethod2Model => new PaymentMethod
         {
            Id = "83562370-3e5c-51db-87da-752af5ab9559",
            Type = "ach_bank_account",
            Name = "International Bank *****1111",
            Currency = "USD",
            PrimaryBuy = true,
            PrimarySell = true,
            AllowBuy = true,
            AllowSell = true,
            AllowDeposit = true,
            AllowWithdraw = true,
            InstantBuy = false,
            InstantSell = false,
            CreatedAt = DateTimeOffset.Parse("2015-01-31T20:49:02Z"),
            UpdatedAt = DateTimeOffset.Parse("2015-02-11T16:53:57-08:00"),
            Resource = "payment_method",
            ResourcePath = "/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559"
      };


      public const string Notification1 = @"  {
      ""id"": ""6bf0ca21-0b2f-5e8a-b95e-7bd7eaccc338"",
      ""type"": ""wallet:buys:completed"",
      ""data"": {
        ""id"": ""67e0eaec-07d7-54c4-a72c-2e92826897df"",
        ""status"": ""completed"",
        ""payment_method"": {
          ""id"": ""83562370-3e5c-51db-87da-752af5ab9559"",
          ""resource"": ""payment_method"",
          ""resource_path"": ""/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559""
        },
        ""transaction"": {
          ""id"": ""441b9494-b3f0-5b98-b9b0-4d82c21c252a"",
          ""resource"": ""transaction"",
          ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a""
        },
        ""amount"": {
          ""amount"": ""1.00000000"",
          ""currency"": ""BTC""
        },
        ""total"": {
          ""amount"": ""10.25"",
          ""currency"": ""USD""
        },
        ""subtotal"": {
          ""amount"": ""10.10"",
          ""currency"": ""USD""
        },
        ""created_at"": ""2015-01-31T20:49:02Z"",
        ""updated_at"": ""2015-02-11T16:54:02-08:00"",
        ""resource"": ""buy"",
        ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/buys/67e0eaec-07d7-54c4-a72c-2e92826897df"",
        ""committed"": true,
        ""instant"": false,
        ""fees"": [
          {
            ""type"": ""coinbase"",
            ""amount"": {
              ""amount"": ""0.00"",
              ""currency"": ""USD""
            }
          },
          {
            ""type"": ""bank"",
            ""amount"": {
              ""amount"": ""0.15"",
              ""currency"": ""USD""
            }
          }
        ],
        ""payout_at"": ""2015-02-18T16:54:00-08:00""
      },
      ""user"": {
        ""id"": ""f01c821e-bb35-555f-a4da-548672963119"",
        ""resource"": ""user"",
        ""resource_path"": ""/v2/users/f01c821e-bb35-555f-a4da-548672963119""
      },
      ""account"": {
        ""id"": ""8d5f086c-d7d5-58ee-890e-c09b3d8d4434"",
        ""resource"": ""account"",
        ""resource_path"": ""/v2/accounts/8d5f086c-d7d5-58ee-890e-c09b3d8d4434""
      },
      ""delivery_attempts"": 0,
      ""created_at"": ""2015-11-10T19:15:06Z"",
      ""resource"": ""notification"",
      ""resource_path"": ""/v2/notifications/6bf0ca21-0b2f-5e8a-b95e-7bd7eaccc338""
    }";

      public static Notification Notification1Model => new Notification
         {
            Id = "6bf0ca21-0b2f-5e8a-b95e-7bd7eaccc338",
            Type = "wallet:buys:completed",
            Data = new JObject
               {
                  {"id", "67e0eaec-07d7-54c4-a72c-2e92826897df"},
                  {"status", "completed"},
                  {
                     "payment_method", new JObject
                        {
                           {"id", "83562370-3e5c-51db-87da-752af5ab9559"},
                           {"resource", "payment_method"},
                           {"resource_path", "/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559"}
                        }
                  },
                  {
                     "transaction", new JObject
                        {
                           {"id", "41b9494-b3f0-5b98-b9b0-4d82c21c252a"},
                           {"resource", "transaction"},
                           {"resource_path", "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a"}
                        }
                  },
                  {
                     "amount", new JObject
                        {
                           {"amount", "1.00000000"},
                           {"currency", "BTC"}
                        }
                  },
                  {
                     "total", new JObject
                        {
                           {"amount", "1.00000000"},
                           {"currency", "BTC"}
                        }
                  },
                  {
                     "subtotal", new JObject
                        {
                           {"amount", "1.00000000"},
                           {"currency", "BTC"}
                        }
                  },
                  {"created_at", "2015-01-31T20:49:02Z"},
                  {"updated_at", "2015-02-11T16:54:02-08:00"},
                  {"resource", "buy"},
                  {"resource_path", "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/buys/67e0eaec-07d7-54c4-a72c-2e92826897df"},
                  {"committed", true},
                  {"instant", false},
                  {
                     "fees", new JArray
                        {
                           new JObject
                              {
                                 {"type", "coinbase"},
                                 {
                                    "amount", new JObject
                                       {
                                          {"amount", "0.00"},
                                          {"currency", "USD"}
                                       }
                                 },
                              },
                           new JObject
                              {
                                 {"type", "bank"},
                                 {
                                    "amount", new JObject
                                       {
                                          {"amount", "0.15"},
                                          {"currency", "USD"}
                                       }
                                 },
                              }
                        }
                  },
                  {"payout_at", "2015-02-18T16:54:00-08:00"},
               },
            User = new Entity
               {
                  Id = "f01c821e-bb35-555f-a4da-548672963119",
                  Resource = "user",
                  ResourcePath = "/v2/users/f01c821e-bb35-555f-a4da-548672963119"
               },
            Account = new Entity
               {
                  Id = "8d5f086c-d7d5-58ee-890e-c09b3d8d4434",
                  Resource = "account",
                  ResourcePath = "/v2/accounts/8d5f086c-d7d5-58ee-890e-c09b3d8d4434"
            },
            DeliveryAttempts = 0,
            CreatedAt = DateTimeOffset.Parse("2015-11-10T19:15:06Z"),
            Resource = "notification",
            ResourcePath = "/v2/notifications/6bf0ca21-0b2f-5e8a-b95e-7bd7eaccc338"
         };


      public const string Notification2 = @"{
    ""id"": ""6bf0ca21-0b2f-5e8a-b95e-7bd7eaccc338"",
    ""type"": ""wallet:buys:completed"",
    ""data"": {
      ""id"": ""67e0eaec-07d7-54c4-a72c-2e92826897df"",
      ""status"": ""completed"",
      ""payment_method"": {
        ""id"": ""83562370-3e5c-51db-87da-752af5ab9559"",
        ""resource"": ""payment_method"",
        ""resource_path"": ""/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559""
      },
      ""transaction"": {
        ""id"": ""441b9494-b3f0-5b98-b9b0-4d82c21c252a"",
        ""resource"": ""transaction"",
        ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a""
      },
      ""amount"": {
        ""amount"": ""1.00000000"",
        ""currency"": ""BTC""
      },
      ""total"": {
        ""amount"": ""10.25"",
        ""currency"": ""USD""
      },
      ""subtotal"": {
        ""amount"": ""10.10"",
        ""currency"": ""USD""
      },
      ""created_at"": ""2015-01-31T20:49:02Z"",
      ""updated_at"": ""2015-02-11T16:54:02-08:00"",
      ""resource"": ""buy"",
      ""resource_path"": ""/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/buys/67e0eaec-07d7-54c4-a72c-2e92826897df"",
      ""committed"": true,
      ""instant"": false,
      ""fee"": {
        ""amount"": ""0.15"",
        ""currency"": ""USD""
      },
      ""payout_at"": ""2015-02-18T16:54:00-08:00""
    },
    ""user"": {
      ""id"": ""f01c821e-bb35-555f-a4da-548672963119"",
      ""resource"": ""user"",
      ""resource_path"": ""/v2/users/f01c821e-bb35-555f-a4da-548672963119""
    },
    ""account"": {
      ""id"": ""8d5f086c-d7d5-58ee-890e-c09b3d8d4434"",
      ""resource"": ""account"",
      ""resource_path"": ""/v2/accounts/8d5f086c-d7d5-58ee-890e-c09b3d8d4434""
    },
    ""delivery_attempts"": 0,
    ""created_at"": ""2015-11-10T19:15:06Z"",
    ""resource"": ""notification"",
    ""resource_path"": ""/v2/notifications/6bf0ca21-0b2f-5e8a-b95e-7bd7eaccc338""
  }";

      public static Notification Notification2Model => new Notification
      {
         Id = "6bf0ca21-0b2f-5e8a-b95e-7bd7eaccc338",
         Type = "wallet:buys:completed",
         Data = new JObject
               {
                  {"id", "67e0eaec-07d7-54c4-a72c-2e92826897df"},
                  {"status", "completed"},
                  {
                     "payment_method", new JObject
                        {
                           {"id", "83562370-3e5c-51db-87da-752af5ab9559"},
                           {"resource", "payment_method"},
                           {"resource_path", "/v2/payment-methods/83562370-3e5c-51db-87da-752af5ab9559"}
                        }
                  },
                  {
                     "transaction", new JObject
                        {
                           {"id", "41b9494-b3f0-5b98-b9b0-4d82c21c252a"},
                           {"resource", "transaction"},
                           {"resource_path", "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/transactions/441b9494-b3f0-5b98-b9b0-4d82c21c252a"}
                        }
                  },
                  {
                     "amount", new JObject
                        {
                           {"amount", "1.00000000"},
                           {"currency", "BTC"}
                        }
                  },
                  {
                     "total", new JObject
                        {
                           {"amount", "10.25"},
                           {"currency", "USD"}
                        }
                  },
                  {
                     "subtotal", new JObject
                        {
                           {"amount", "10.10"},
                           {"currency", "USD"}
                        }
                  },
                  {"created_at", "2015-01-31T20:49:02Z"},
                  {"updated_at", "2015-02-11T16:54:02-08:00"},
                  {"resource", "buy"},
                  {"resource_path", "/v2/accounts/2bbf394c-193b-5b2a-9155-3b4732659ede/buys/67e0eaec-07d7-54c4-a72c-2e92826897df"},
                  {"committed", true},
                  {"instant", false},
                  {"fee",new JObject
                        {
                           { "amount", "0.15"},
                           {"currency", "USD" }
                        }},
                  {"payout_at", "2015-02-18T16:54:00-08:00"},
               },
         User = new Entity
         {
            Id = "f01c821e-bb35-555f-a4da-548672963119",
            Resource = "user",
            ResourcePath = "/v2/users/f01c821e-bb35-555f-a4da-548672963119"
         },
         Account = new Entity
         {
            Id = "8d5f086c-d7d5-58ee-890e-c09b3d8d4434",
            Resource = "account",
            ResourcePath = "/v2/accounts/8d5f086c-d7d5-58ee-890e-c09b3d8d4434"
         },
         DeliveryAttempts = 0,
         CreatedAt = DateTimeOffset.Parse("2015-11-10T19:15:06Z"),
         Resource = "notification",
         ResourcePath = "/v2/notifications/6bf0ca21-0b2f-5e8a-b95e-7bd7eaccc338"
      };


   }


}
