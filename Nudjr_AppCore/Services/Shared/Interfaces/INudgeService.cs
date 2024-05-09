using Nudjr_Domain.Models.Dtos;
using Nudjr_Domain.Models.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.Shared.Interfaces
{
    public interface INudgeService
    {
        Task<NudgeDto> FetchNudgeFromGeminiAsync(NudgeDataModel nudgeDataModel);
    }
}
