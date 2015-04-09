using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coinbase.ObjectModel
{
    public abstract class CoinbaseResponse
    {
        public bool Success { get; set; }

        public string[] Errors { get; set; }

        [JsonProperty]
        internal string Error
        {
            set
            {
                if( this.Errors == null )
                {
                    this.Errors = new[] {value};
                }
                else
                {
                    this.Errors = new List<string>(this.Errors)
                        {
                            value
                        }.ToArray();
                }
            }
        }
    }

}