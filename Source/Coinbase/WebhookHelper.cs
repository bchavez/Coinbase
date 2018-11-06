
using System;
using System.Security.Cryptography;
using System.Text;

namespace Coinbase
{
   public static class WebhookHelper
   {
      private static UTF8Encoding safeUtf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier:false, throwOnInvalidBytes: true);

      public const string CoinbasePublicKey =
         "<RSAKeyValue><Modulus>9MsJBuXzFGIh/xkAA9CyQdZKRerV+apyOAWY7sEYV/AJg+AX/tW2SHeZj+3OilNYm5DlBi6ZzDboczmENrFnmUXQsecsR5qjdDWb2qYqBkDkoZP02m9o9UmKObR8coKW4ZBw0hEf3fP9OEofG2s7Z6PReWFyQffnnecwXJoN22qjjsUtNNKOOo7/l+IyGMVmdzJbMWQS4ybaU9r9Ax0J4QUJSS/S4j4LP+3Z9i2DzIe4+PGa4Nf7fQWLwE45UUp5SmplxBfvEGwYNEsHvmRjusIy2ZunSO2CjJ/xGGn9+/57W7/SNVzk/DlDWLaN27hUFLEINlWXeYLBPjw5GGWpieXGVcTaFSLBWX3JbOJ2o2L4MxinXjTtpiKjem9197QXSVZ/zF1DI8tRipsgZWT2/UQMqsJoVRXHveY9q9VrCLe97FKAUiohLsskr0USrMCUYvLU9mMw15hwtzZlKY8TdMH2Ugqv/CPBuYf1Bc7FAsKJwdC504e8kAUgomi4tKuUo25LPZJMTvMTs/9IsRJvI7ibYmVR3xNsVEpupdFcTJYGzOQBo8orHKPFn1jj31DIIKociCwu6m8ICDgLuMHj7bUHIlTzPPT7hRPyBQ1KdyvwxbguqpNhqp1hG2sghgMr0M6KMkUEz38JFElsVrpF4z+EqsFcIZzjkSG16BjjjTk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

      /// <summary>
      /// Validate a callback from Coinbase.
      /// </summary>
      /// <param name="postBody">HTTP POST body</param>
      /// <param name="headerValue">The signature to be verified is present in the ‘CB-SIGNATURE’ HTTP Header encoded as base64</param>
      public static bool IsValid(string postBody, string headerValue)
      {
         var data = safeUtf8.GetBytes(postBody);
         var signature = Convert.FromBase64String(headerValue);

         using (var rsa = new RSACryptoServiceProvider())
         using (var sha256 = SHA256.Create())
         {
            rsa.FromXmlString(CoinbasePublicKey);
            return rsa.VerifyData(data, sha256, signature);
         }
      }
   }
}