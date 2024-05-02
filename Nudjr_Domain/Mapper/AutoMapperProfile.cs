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
                    viewModel.Notifications = GetSortedNofications(entity.Content);
                });
        }

        private Dictionary<string, string> GetSortedNofications(string text)
        {
            string[] splitInput = text.Split(new string[] { "\n\n " }, StringSplitOptions.None);
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (string s in splitInput)
            {
                // Find the index of the first colon
                int colonIndex = s.IndexOf('.');

                // Get the key and value
                string key = s.Substring(0, colonIndex).Trim();
                string value = s.Substring(colonIndex + 1).Trim();

                // Add the key and value to the dictionary
                keyValuePairs.Add(key, value);
            }

            return keyValuePairs;
        }
    }
}
