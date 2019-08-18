using System;
using Flurl;
using Flurl.Http;

namespace Coinbase
{
   public static class ExtensionsForUrl
   {
      public static IFlurlRequest AppendPathSegmentsRequire(this IFlurlRequest req, params object[] segments)
      {
         req.Url.AppendPathSegmentsRequire(segments);
         return req;
      }
      public static Url AppendPathSegmentsRequire(this string url, params object[] segments)
      {
         return new Url(url).AppendPathSegmentsRequire(segments);
      }
      public static Url AppendPathSegmentsRequire(this Url url, params object[] segments)
      {
         foreach( var segment in segments )
         {
            if( segment is string s && string.IsNullOrWhiteSpace(s) )
            {
               throw new ArgumentException("Part of the URL segment is null or whitespace. Check the parameters to the method you are calling and verify none of the arguments are null or whitespace.");
            }

            url.AppendPathSegment(segment);
         }

         return url;
      }
   }
}
