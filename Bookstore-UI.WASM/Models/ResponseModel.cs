﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore_UI.WASM.Models
{
    public class ResponseModel
    {
        public string Content { get; set; }

        public bool Success { get; set; }

        public string Error { get; set; }
    }
}
