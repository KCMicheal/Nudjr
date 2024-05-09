using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Entities
{
    public class ALARM : BASE_ENTITY
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime AlarmDateTime { get; set; }
        public DateTime AlarmTimestamp { get; set; }
        public string? AlarmTitle { get; set; }
        public string? AlarmMessage { get; set; }
        public bool IsActive {  get; set; }


        [ForeignKey("UserId")]
        public virtual USER? User {  get; set; }  
    }
}
