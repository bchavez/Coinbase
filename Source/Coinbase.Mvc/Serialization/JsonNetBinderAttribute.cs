using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Coinbase.Mvc.Serialization
{
    [Obsolete("The Coinbase.Mvc DLL has been deprecated. This specialized MVC DLL is no longer needed. Please use the main Coinbase library here: https://www.nuget.org/packages/Coinbase/. The JsonNetBinderAttribute is no longer needed. For MVC projects: Parse the Request.InputStream as a string and the pass the JSON string into api.GetNotification() to get the Notification callback object. For Web API: Simply mount the Notification class as an argument model to your callback API endpoint. You must still verify the callback is from Coinbase.", true)]
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