using System.ComponentModel.DataAnnotations;

namespace Nudjr_Domain.Models.Dtos
{
    public class AlarmDto
    {
        public DateTime AlarmDateTime { get; set; }
        public long AlarmTimestamp { get; set; }
        public string? AlarmTitle { get; set; }
        public string? AlarmMessage { get; set; }
        public bool IsActive { get; set; }
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
