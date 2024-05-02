using Nudjr_Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Entities
{
    public class USER_ACTIVITY_LOG
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ActivityType ActivityType { get; set; }
        public string? ActivityDetails { get; set; }
        public double? Timestamp { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual USER User { get; set; }
    }
}
