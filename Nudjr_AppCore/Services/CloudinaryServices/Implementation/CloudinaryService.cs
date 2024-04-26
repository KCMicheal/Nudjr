using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Nudjr_AppCore.Services.CacheServices.Configuration;
using Nudjr_AppCore.Services.CloudinaryServices.Configuration;
using Nudjr_AppCore.Services.CloudinaryServices.Dtos;
using Nudjr_AppCore.Services.CloudinaryServices.Interfaces;
using Nudjr_Domain.Models.UtilityModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.CloudinaryServices.Implementation
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;
       

        public CloudinaryService(IConfiguration Configuration)
        {
            _configuration = Configuration;
            CloudinaryConfig _config = _configuration.GetSection("CloudinaryConfig").Get<CloudinaryConfig>();
            Account account = new(_config.CloudName, _config.ApiKey, _config.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
            
        }

        public async Task<DeletionResult> DeleteObjectAsync(string publicId)
        {
            DeletionParams deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }

        public async Task<ImageUploadResult> UploadImageAsync(FileUploadDto model)
        {
            if (model is null)
            {
                return null!;
            }

            if (model.File.Length <= 0)
            {
                return null!;
            }

            using Stream stream = model.File.OpenReadStream();

            ImageUploadParams uploadParams = new()
            {
                File = new FileDescription(model.File.FileName, stream),
                Transformation = model.Transformation,
                PublicId = model.PublicId,
                Folder = model.Folder,
                Tags = model.Tags
            };
            return await _cloudinary.UploadAsync(uploadParams);
        }

        public Task<RawUploadResult> UploadResultDocumentAsync(FileUploadDto model)
        {
            if (model is null)
            {
                return null!;
            }
            if (model.File.Length <= 0)
            {
                return null!;
            }

            using Stream stream = model.File.OpenReadStream();
            RawUploadParams uploadParams = new()
            {
                File = new FileDescription(model.File.FileName, stream),
                
            };

            return _cloudinary.UploadAsync(uploadParams);
        }

        public async Task<VideoUploadResult> UploadVideoAsync(FileUploadDto model)
        {
           if(model is null)
            {
                return null!;
            }
           if (model.File.Length <= 0)
            {
                return null!;
            }
            using Stream stream = model.File.OpenReadStream();
            var uploadParam = new VideoUploadParams()
            {
                File = new FileDescription(model.File.FileName, stream),
                Transformation = model.Transformation,
                PublicId = model.PublicId,
                Overwrite = true
            };
            
            return await _cloudinary.UploadAsync(uploadParam);   

        }
    }
}
