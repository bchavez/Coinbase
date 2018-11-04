using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinbase.ObjectModel
{
    public class RequestOptions
    {
        /// <summary>
        /// The API endpoint. Ex: /checkout, /orders, /time
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The HTTP method to use. Default: POST
        /// </summary>
        public Method HttpMethod { get; set; } = Method.POST;
        //string endpoint, object body, Method httpMethod = Method.POST

        /// <summary>
        /// List Of Request Parameters
        /// </summary>
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        /// <summary>
        /// Initializes the Request Options
        /// </summary>
        public RequestOptions(string endpoint, Method httpMethod = Method.POST)
        {

            this.Endpoint = endpoint;
            this.HttpMethod = httpMethod;
        }

        /// <summary>
        /// Adds a parameter based on the type of request your making e.g (GET: Querystring, POST: body)
        /// </summary>
        /// <param name="name">The name/key of the parameter</param>
        /// <param name="value"> the parameter value</param>
        public void AddParameter(string name, string value)
        {
            this.AddParameter(name, value, ParameterType.GetOrPost);
        }

        /// <summary>
        /// Adds a Header to your request
        /// </summary>
        /// <param name="name">The name/key of the parameter</param>
        /// <param name="value"> the parameter value</param>
        public void AddHeader(string name, string value)
        {
            this.AddParameter(name, value, ParameterType.HttpHeader);
        }

        /// <summary>
        /// Adds a parameter to the request
        /// </summary>
        /// <param name="name">The name/key of the parameter</param>
        /// <param name="value"> the parameter value</param>
        /// <param name="type">Parameter Type: e.g Querystring, Body, Header</param>
        public void AddParameter(string name, string value, ParameterType type)
        {
            var parameter = new Parameter();
            parameter.Type = type;
            parameter.Name = name;
            parameter.Value = value;
            this.Parameters.Add(parameter);
        }
    }
}
