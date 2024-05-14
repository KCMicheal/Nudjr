using AutoMapper;
using Nudjr_AppCore.Services.Extensions;
using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_Domain.Entities;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.Dtos;
using Nudjr_Domain.Models.ServiceModels;
using Nudjr_Persistence.UnitOfWork.Interfaces;

namespace Nudjr_AppCore.Services.Shared.Services
{
    public class AlarmService : BaseEntityService, IAlarmService
    {
        private readonly IServiceFactory ServiceFactory;
        public AlarmService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            ServiceFactory = serviceFactory;
        }

        public async Task<ServiceOperationModel<AlarmDto>> CreateAlarm(CreateAlarmDto model, USER user)
        {
            INudgeService _nudgeService = ServiceFactory.GetService<INudgeService>();
            ServiceOperationModel<AlarmDto> alarmModel;
            if (model is null) throw new InvalidOperationException("Alarm model can't be empty");

            ALARM alarm = await _unitOfWork.GetRepository<ALARM>().SingleOrDefaultAsync(x => x.UserId == user.Id && x.AlarmTitle == model.AlarmTitle);
            if (alarm is not null)
                throw new InvalidOperationException("Alarm already exist, try a new title");

            DateTime currentTime = DateTime.UtcNow;

            alarm = new ALARM()
            {
                AlarmDateTime = model.AlarmDateTime,
                AlarmTimestamp = model.AlarmTimestamp.ConvertToDate(),
                AlarmTitle = model.AlarmTitle,
                AlarmMessage = model.AlarmMessage,
                IsActive = model.IsActive,
                CreatedAt = currentTime,
                UpdatedAt = currentTime,
                EntityStatus = EntityStatus.ACTIVE,
                UserId = user.Id
            };


            _unitOfWork.GetRepository<ALARM>().Add(alarm);
            int rowCount = await _unitOfWork.SaveChangesAsync();

            if (rowCount > 0)
            {
                NudgeDataModel nudge = new NudgeDataModel()
                {
                    Task = $"{alarm.AlarmTitle}: {alarm.AlarmMessage}",
                    TaskDate = alarm.AlarmDateTime,
                    NumberOfNudges = user.NumberOfNudges,
                    Theme = model.Nudge.Theme,
                    User = user,
                };

                var fetchedNudges = await _nudgeService.FetchNudgeFromGeminiAsync(nudge);
                var alarmDto = _mapper.Map<AlarmDto>(model);
                return alarmModel = new ServiceOperationModel<AlarmDto>
                {
                    Data = alarmDto,
                    SuccessMessage = "Alarm & Nudge Created Successfully",
                    IsSuccess = true
                };
            }

            return alarmModel = new ServiceOperationModel<AlarmDto>
            {
                Data = null,
                SuccessMessage = "Alarm & Nudge Creation Failed",
                IsSuccess = false
            };
        }
    }
}
