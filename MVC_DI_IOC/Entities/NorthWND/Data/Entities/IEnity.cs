﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_DI_IOC.Core.NorthWND.Data.Entities
{
    public interface IEnity<TPrimaryKey>
    {
        TPrimaryKey ID { get; set; }
    }
}
