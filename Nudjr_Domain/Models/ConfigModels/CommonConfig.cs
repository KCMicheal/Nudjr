using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.ConfigModels
{
    public class CommonConfig
    {
        public int MaxFailedLoginAttemptCount { get; set; }
        public int FailedLoginLockoutDurationInMinutes { get; set; }
        public int DefaultDeadlineForValidatedOTPUsageInMinutes { get; set; }
    }
}
