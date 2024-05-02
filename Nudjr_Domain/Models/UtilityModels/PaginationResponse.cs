using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.UtilityModels
{
    public record PaginationResponse<T>(int PageSize, int CurrentPage, int TotalPages, int TotalRecords, IEnumerable<T> Records) : BaseRecord where T : BaseRecord;
}
