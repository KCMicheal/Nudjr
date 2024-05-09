using Nudjr_Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.Dtos
{
    public class NudgeDto
    {
        public Guid UserId { get; set; }
        public Dictionary<string, string>? Notifications { get; set; }
        public string? Theme { get; set; }
    }

    public class CreateNudgeDto
    {
        public MotivationalTheme Theme { get; set; }

    }
}
