﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.Shared.Interfaces
{
    public interface IServiceFactory
    {
        object GetService(Type type);
        T GetService<T>() where T : class;
    }
}
