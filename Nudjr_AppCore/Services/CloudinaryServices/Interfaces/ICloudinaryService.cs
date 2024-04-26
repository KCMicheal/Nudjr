using CloudinaryDotNet.Actions;
using Nudjr_AppCore.Services.CloudinaryServices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.CloudinaryServices.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(FileUploadDto model);
        Task<VideoUploadResult> UploadVideoAsync(FileUploadDto model);
        Task<RawUploadResult> UploadResultDocumentAsync(FileUploadDto model);

        Task<DeletionResult> DeleteObjectAsync(string publicId);
    }
}
