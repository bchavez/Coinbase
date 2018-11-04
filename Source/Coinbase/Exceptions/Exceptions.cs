using Coinbase.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Coinbase.Exceptions
{
    public class CoinbaseApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public ErrorResponse ErrorResponse { get; set; }
        public CoinbaseApiException(HttpStatusCode statusCode,  ErrorResponse errorResponse)
        {
            this.StatusCode = statusCode;
            this.ErrorResponse = errorResponse;
        }
    }

    public class TokenExpiredException : CoinbaseApiException
    {
        public TokenExpiredException(HttpStatusCode statusCode, ErrorResponse errorResponse) 
            : base(statusCode, errorResponse)
        {
        }

        public override string Message => "Token Has Expired";
    }

    public class InvalidScopeException : CoinbaseApiException
    {
        public InvalidScopeException(HttpStatusCode statusCode, ErrorResponse errorResponse) 
            : base(statusCode, errorResponse)
        {
        }

        public override string Message => "Forbidden, Check your scopes to make sure you have Access Priviledges";
    }

    public class Transaction2FaRequiredException : CoinbaseApiException
    {
        public Transaction2FaRequiredException(HttpStatusCode statusCode, ErrorResponse errorResponse)
            : base(statusCode, errorResponse)
        {
        }

        public override string Message => "Two factor authentication is required, replay the request with the 2FA Token";
    }
}
