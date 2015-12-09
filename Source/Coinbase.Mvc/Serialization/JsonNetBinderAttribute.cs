using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Coinbase.Mvc.Serialization
{
    [Obsolete("The Coinbase.Mvc DLL has been deprecated. This specialized MVC DLL is no longer needed. Please use the main Coinbase library here: https://www.nuget.org/packages/Coinbase/. The JsonNetBinderAttribute (when used with Notification model for callbacks) is no longer needed. Simply replace CoinbaseCallback with the Notification model (without any special model binder) like any other normal model.", true)]
    public class JsonNetBinderAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new JsonModelBinder();
        }

        public class JsonModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                try
                {
                    if (
                        !controllerContext.HttpContext.Request.ContentType.StartsWith("application/json",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                    controllerContext.HttpContext.Request.InputStream.Seek(0, SeekOrigin.Begin);
                    using (var sr = new StreamReader(controllerContext.HttpContext.Request.InputStream))
                    {
                        string json = sr.ReadToEnd();
                        //var json = controllerContext.HttpContext.Request.Form[bindingContext.ModelName];
                        // Swap this out with whichever JSON deserializer you prefer.
                        return JsonConvert.DeserializeObject(json, bindingContext.ModelType);
                    }
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}