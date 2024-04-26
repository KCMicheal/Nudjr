using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.ExceptionModels
{
    public class AppException : Exception
    {
        public AppException(string message) : base(message)
        {

        }
    }
}
