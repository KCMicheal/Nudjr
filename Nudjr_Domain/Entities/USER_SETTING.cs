using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Entities
{
    public class USER_SETTING : BASE_ENTITY
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? PreferredLanguage { get; set; }


        [ForeignKey("UserId")]
        public virtual USER User { get; set; }
    }
}
