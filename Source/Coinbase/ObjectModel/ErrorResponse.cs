﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinbase.ObjectModel
{
    public class ErrorResponse
    {
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string Id { get; set; }
        public string Message { get; set; }
    }
}
