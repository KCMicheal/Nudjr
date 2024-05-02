using AutoMapper;
using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_Persistence.UnitOfWork.Interfaces;

namespace Nudjr_AppCore.Services.Shared.Services
{
    public class AlarmService : BaseEntityService, IAlarmService
    {
        private readonly INudgeService _nudgeService;
        public AlarmService(INudgeService nudgeService, IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

            _nudgeService = nudgeService;

        }
    }
}
