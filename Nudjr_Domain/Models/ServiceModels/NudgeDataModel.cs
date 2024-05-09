using Nudjr_Domain.Entities;
using Nudjr_Domain.Enums;

namespace Nudjr_Domain.Models.ServiceModels
{
    public class NudgeDataModel
    {
        public USER? User { get; set; }
        public NumberOfNudges NumberOfNudges { get; set; }
        public MotivationalTheme Theme { get; set; }
        public string? Task { get; set; }
        public DateTime TaskDate { get; set; }
    }
}
