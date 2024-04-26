using Nudjr_Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Entities
{
    public class BASE_ENTITY
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public EntityStatus EntityStatus { get; set; }
    }
}
