using AutoMapper;
using Microsoft.Extensions.Options;
using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_Domain.Entities;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.ConfigModels;
using Nudjr_Domain.Models.Dtos;
using Nudjr_Domain.Models.ServiceModels;
using Nudjr_Persistence.UnitOfWork.Interfaces;

namespace Nudjr_AppCore.Services.Shared.Services
{
    public class NudgeService : BaseEntityService, INudgeService
    {
        private readonly IHttpClientProvider _httpClientProvider;
        private readonly GeminiConfig _geminiConfig;
        private readonly PromptConfig _promptConfig;
        public NudgeService(IHttpClientProvider httpClientProvider, IOptionsSnapshot<GeminiConfig> geminiConfig, IOptionsSnapshot<PromptConfig> promptConfig,
            IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _httpClientProvider = httpClientProvider;
            _geminiConfig = geminiConfig.Value;
            _promptConfig = promptConfig.Value;
        }

        public async Task<NudgeDto> FetchNudgeFromGeminiAsync(NudgeDataModel nudgeDataModel)
        {
            try
            {
                string url = string.Format(_geminiConfig.BaseUrl, _geminiConfig.ApiKey);
                int age = CalculateAge((DateTime)nudgeDataModel.User.DateOfBirth, DateTime.UtcNow);
                var gender = Gender(nudgeDataModel.User.Gender);
                string bodyPrompt = string.Format(_promptConfig.Prompt, nudgeDataModel.User.FirstName, nudgeDataModel.User.LastName, nudgeDataModel.User.PersonalityType.ToString(),
                    age, nudgeDataModel.NumberOfNudges, gender.ToLower(), nudgeDataModel.Task, gender.ToLower(), nudgeDataModel.TaskDate.ToShortDateString(),
                    nudgeDataModel.TaskDate.ToShortTimeString(), nudgeDataModel.Theme);
                var body = new Nudjr_Domain.Models.ServiceModels.Prompt.PromptRequestModel
                {
                    contents = new List<Nudjr_Domain.Models.ServiceModels.Prompt.Content>
                    {
                        new Nudjr_Domain.Models.ServiceModels.Prompt.Content
                        {
                            parts = new List<Nudjr_Domain.Models.ServiceModels.Prompt.Part>
                            {
                                new Nudjr_Domain.Models.ServiceModels.Prompt.Part
                                {
                                    text = bodyPrompt
                                }
                            }
                        }
                    }
                };

                HttpResponseMessage response = await _httpClientProvider.DoHttpPostAsync(url, body, null);
                if (response.IsSuccessStatusCode)
                {
                    var responseMessage = await _httpClientProvider.ParseNudjrApiResponse<PromptResponseModel>(response);
                    DateTime currentDateTime = DateTime.UtcNow;
                    NUDGE nudge = new NUDGE()
                    {
                        CreatedAt = currentDateTime,
                        UpdatedAt = currentDateTime,
                        EntityStatus = EntityStatus.ACTIVE,
                        UserId = nudgeDataModel.User.Id,
                        Content = responseMessage.Candidates[0].Content.Parts[0].Text,
                        Tone = nudgeDataModel.Theme.ToString(),
                    };

                    _unitOfWork.GetRepository<NUDGE>().Add(nudge);
                    int row = await _unitOfWork.SaveChangesAsync();
                    if (row > 0)
                    {
                        NudgeDto nudgeDto = _mapper.Map<NudgeDto>(nudge);
                        return nudgeDto;
                    }
                }

                return new NudgeDto { };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public int CalculateAge(DateTime dateOfBirth, DateTime currentDate)
        {
            int age = currentDate.Year - dateOfBirth.Year;

            // Check if the birthday has occurred this year
            if (currentDate.Month < dateOfBirth.Month || (currentDate.Month == dateOfBirth.Month && currentDate.Day < dateOfBirth.Day))
            {
                age--;
            }

            return age;
        }

        public string Gender(Gender gender)
        {
            string genderName;
            switch (gender)
            {
                case Nudjr_Domain.Enums.Gender.Male:
                    genderName = "He";
                    break;
                case Nudjr_Domain.Enums.Gender.Female:
                    genderName = "She";
                    break;
                case Nudjr_Domain.Enums.Gender.Other:
                    genderName = "Other";
                    break;
                default:
                    throw new InvalidOperationException("Gender Not Specified");
            }

            return genderName;
        }
    }
}
