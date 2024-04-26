using Nudjr_Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.ViewModels
{
    public class AdminRoleRequestModel
    {
        public Guid UserId { get; set; }
        public string? Role { get; set; }
    }

    public class RemoveRoleResponseModel
    {
        public bool IsSuccess { get; set; }
        public PersonType RemainingRole { get; set; }
    }
}
