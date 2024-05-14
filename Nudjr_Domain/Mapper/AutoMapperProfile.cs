using AutoMapper;
using Nudjr_Domain.Entities;
using Nudjr_Domain.Models.Dtos;

namespace Nudjr_Domain.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<NUDGE, NudgeDto>()
                .ForMember(x => x.Theme, y => y.MapFrom(y => y.Tone))
                .AfterMap((entity, viewModel) =>
                {
                    viewModel.Notifications = SplitResponse(entity.Content);
                });


            CreateMap<ALARM, AlarmDto>().
                AfterMap((entity, viewModel) =>
            {
                viewModel.AlarmTimestamp = ConvertToTimestamp(entity.AlarmTimestamp);
            }).ReverseMap();

            CreateMap<CreateAlarmDto, AlarmDto>();
        }


        private Dictionary<string, string> SplitResponse(string response)
        {
            Dictionary<string, string> answers = new Dictionary<string, string>();

            string[] lines = response.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                int dotIndex = line.IndexOf('.');
                if (dotIndex != -1)
                {
                    string answerNumber = line.Substring(0, dotIndex).Trim();
                    string answerText = line.Substring(dotIndex + 1).Trim();
                    answers.Add(answerNumber, answerText);
                }
            }

            return answers;
        }

        private long ConvertToTimestamp(DateTime dateTime)
        {
            long timestamp = (long)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return timestamp;
        }
    }
}
