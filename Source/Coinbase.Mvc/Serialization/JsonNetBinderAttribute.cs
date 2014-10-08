using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Coinbase.Serialization
{
    /*
* Example 

Example Business Objects

public class Person
{
public string Name { get; set; }
public int Age { get; set; }
// Note: This property is not a primitive!
public DomesticAnimal Pet { get; set; }
}
public class DomesticAnimal
{
public string Name { get; set; }
public int Age { get; set; }
public string Species { get; set; }
}

Example Javascript Code

var person = {
Name : 'Tom',
Age : 23,
Pet : {
    Name : 'Buddy',
    Age : 12,
    Species : 'Canine'
}
};
var url = 'Person/Update';

function jQueryTest() {
var personJson = $.toJSON(person);
$.post(url, { person : personJson });
}

function extJsTest() {
var personJson = Ext.util.JSON.encode(person);
Ext.Ajax.request({
        url : url,
        params : { person : personJson }
    });
}

Serialized JSON Post Param

person {"Name":"Tom","Age":23,Pet:{"Name":"Buddy",Age:12,Species:"Canine"}}

Example Controller (Where the magic happens!)

public class PersonController : Controller
{
// Note: The JsonBinder attribute has been added to the person parameter.
[AcceptVerbs(HttpVerbs.Post)]
public ActionResult Update([JsonBinder]Person person)
{
    // Both the person and its internal pet object have been populated!
    ViewData["PetName"] = person.Pet.Name;
    return View();
}
}
 
*/

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