using Nudjr_Domain.Entities;
using Nudjr_Domain.Enums;

namespace Nudjr_Domain.Models.ServiceModels
{
    public class NudgeDataModel
    {
        public required USER user { get; set; }
        public NumberOfNudges numberOfNudges { get; set; }
        public MotivationalTheme theme { get; set; }
        public required string task { get; set; }
        public required DateTime taskDate { get; set; }
    }
}
