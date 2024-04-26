using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.CacheServices.Configuration
{
    public class RedisConfig
    {
        public string InstanceId { get; set; } = null!;
        public string Host { get; set; } = null!;
        public string IP { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Port { get; set; }
    }
}
