﻿using Bookstore_UI.WASM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore_UI.WASM.Contracts
{
    public interface IBookRepository: IBaseRepository<Book>
    {
    }
}
