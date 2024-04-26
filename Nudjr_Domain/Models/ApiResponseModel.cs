using Nudjr_Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models
{
    public class ApiResponseModel<TEntity>
    {
        public TEntity? Data { get; set; }
        public ResponseStatus Status { get; set; }
        public string? Message { get; set; }
    }
}
