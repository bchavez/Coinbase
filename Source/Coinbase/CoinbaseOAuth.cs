using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Authenticators;

namespace Coinbase
{
    public class CoinbaseOAuth : CoinbaseApiBase
    {
        protected CoinbaseOAuthOptions options;
        public CoinbaseOAuth(CoinbaseOAuthOptions options) : base(options)
        {
            this.options = options;
        }

        protected override IAuthenticator GetAuthenticator()
        {
            return new CoinbaseOAuthAuthenticator(options);
        }
    }
}
