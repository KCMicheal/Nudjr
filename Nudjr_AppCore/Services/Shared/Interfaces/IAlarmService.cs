using Nudjr_Domain.Entities;
using Nudjr_Domain.Models.Dtos;
using Nudjr_Domain.Models.ServiceModels;

namespace Nudjr_AppCore.Services.Shared.Interfaces
{
    public interface IAlarmService
    {
        Task<ServiceOperationModel<AlarmDto>> CreateAlarm(CreateAlarmDto model, USER user);
    }
}
