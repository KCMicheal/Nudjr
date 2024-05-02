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

        public async Task<NudgeDto> FetchNudgeFromGemini(NudgeDataModel nudgeDataModel)
        {
            try
            {
                string url = string.Format(_geminiConfig.BaseUrl, _geminiConfig.ApiKey);
                int age = CalculateAge((DateTime)nudgeDataModel.user.DateOfBirth, DateTime.UtcNow);
                var gender = Gender(nudgeDataModel.user.Gender);
                string bodyPrompt = string.Format(_promptConfig.Prompt, nudgeDataModel.user.FirstName, nudgeDataModel.user.LastName, age, gender, nudgeDataModel.task,
                    nudgeDataModel.user.Gender, nudgeDataModel.taskDate.ToShortDateString(), nudgeDataModel.taskDate.ToShortTimeString(), nudgeDataModel.theme);
                var body = new Nudjr_Domain.Models.ServiceModels.Prompt.Content()
                {
                    Parts = new List<Nudjr_Domain.Models.ServiceModels.Prompt.Part>()
                    {
                        new Nudjr_Domain.Models.ServiceModels.Prompt.Part()
                        {
                            Text = bodyPrompt
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
                        UserId = nudgeDataModel.user.Id,
                        Content = responseMessage.Candidates[0].Content.Parts[0].Text,
                        Tone = nudgeDataModel.theme.ToString(),
                    };

                    nudge = _unitOfWork.GetRepository<NUDGE>().Add(nudge);
                    int row = await _unitOfWork.SaveChangesAsync();
                    if (row > 0)
                    {
                        NudgeDto nudgeDto = _mapper.Map<NudgeDto>(nudge);
                        return nudgeDto;
                    }
                }

                return null;
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
