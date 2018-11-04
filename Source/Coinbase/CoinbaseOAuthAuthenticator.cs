using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinbase
{
    public class CoinbaseOAuthAuthenticator : IAuthenticator
    {
        //TODO: Support Refresh Tokens
        protected CoinbaseOAuthOptions options;

        public CoinbaseOAuthAuthenticator(CoinbaseOAuthOptions options)
        {
            this.options = options;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddHeader("Authorization", $"Bearer {options.AccessToken}")
                   .AddHeader(CoinbaseConstants.CBVersionHeader, CoinbaseConstants.ApiVersionDate);
        }
    }
}
