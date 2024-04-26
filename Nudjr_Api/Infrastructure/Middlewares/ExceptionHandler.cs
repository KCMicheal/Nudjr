using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.ExceptionModels;
using Nudjr_Domain.Models.ResposneModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Nudjr_Api.Infrastructure.Middlewares
{
    public static class ExceptionHandler
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger)
        {

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";


                    IExceptionHandlerFeature? contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        if (contextFeature.Error.GetType() == typeof(InvalidOperationException) ||
                            contextFeature.Error.GetType() == typeof(NudjrAPIException) ||
                            contextFeature.Error.GetType() == typeof(NotFoundException) ||
                            contextFeature.Error.GetType() == typeof(ArgumentException) ||
                            contextFeature.Error.GetType() == typeof(SecurityTokenException))

                        {

                            await context.Response.WriteAsync(new ErrorDetails
                            {
                                Status = ResponseStatus.APP_ERROR,
                                Message = contextFeature.Error.Message

                            }.ToString());

                        }
                        else
                        {
                            await context.Response.WriteAsync(new ErrorDetails
                            {
                                Status = ResponseStatus.FATAL_ERROR,
                                Message = "Oops, Something Went Wrong"
                            }.ToString());
                        }
                    }
                });
            });
        }

    }
}
