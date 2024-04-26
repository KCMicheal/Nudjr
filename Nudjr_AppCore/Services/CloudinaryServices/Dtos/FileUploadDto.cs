using CloudinaryDotNet;
using Nudjr_Domain.Models.UtilityModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.CloudinaryServices.Dtos
{
    public record FileUploadDto(IFormFile File, Transformation? Transformation, string Folder, string PublicId, string Tags) : BaseRecord;

}
