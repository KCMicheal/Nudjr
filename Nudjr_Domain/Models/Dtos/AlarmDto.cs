using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.Dtos
{
    public class AlarmDto
    {
        
    }

    public class CreateAlarmDto
    {
        [Required]
        public string? AlarmTitle { get; set; }
        [Required]
        public DateTime AlarmDateTime { get; set; }
        public long AlarmTimestamp { get; set; }
        public string? AlarmMessage { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public CreateNudgeDto? Nudge { get; set; }
    }
}
