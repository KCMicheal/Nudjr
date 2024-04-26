using Newtonsoft.Json;
using Nudjr_Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.ResposneModels
{
    public class ErrorDetails
    {
        public ResponseStatus Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
