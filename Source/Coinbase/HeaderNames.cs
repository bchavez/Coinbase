using System.ComponentModel;

namespace Coinbase
{
   public static class HeaderNames
   {
      public const string AccessKey = "CB-ACCESS-KEY";
      public const string AccessSign = "CB-ACCESS-SIGN";
      public const string AccessTimestamp = "CB-ACCESS-TIMESTAMP";
      public const string Version = "CB-VERSION";
      public const string NotificationSignature = "CB-SIGNATURE";
      public const string TwoFactorToken = "CB-2FA-Token";
   }

   public class PaginationOptions
   {
      public int? Limit { get; set; }
      public string Order { get; set; }
      public string StartingAfter { get; set; }
      public string EndingBefore { get; set; }
   }

   [EditorBrowsable(EditorBrowsableState.Never)]
   public static class ExtensionsForFlurlUrl
   {

      [EditorBrowsable(EditorBrowsableState.Never)]
      public static Flurl.Url WithPagination(this Flurl.Url url, PaginationOptions opts)
      {
         if( opts is null ) return url;

         return url.SetQueryParam("limit", opts.Limit)
            .SetQueryParam("order", opts.Order)
            .SetQueryParam("starting_after", opts.StartingAfter)
            .SetQueryParam("ending_before", opts.EndingBefore);
      }
   }
}
