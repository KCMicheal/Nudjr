using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Entities
{
    public class EVENT : BASE_ENTITY
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime EventDateTime { get; set; }
        public string? EventTitle { get; set; }
        public string? EventDescription { get; set; }
        public string? EventLink { get; set; }
        public required bool IsCompleted { get; set; }

        [ForeignKey("UserId")]
        public virtual USER User { get; set; }
    }
}
