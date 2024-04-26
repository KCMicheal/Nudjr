using Microsoft.EntityFrameworkCore;
using Nudjr_Api.Infrastructure.StartupExtensions;
using Nudjr_AppCore.Services.Extensions;
using Nudjr_Domain.Context;

var builder = WebApplication.CreateBuilder(args);
IConfiguration Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddCors(options =>
              options.AddPolicy("CorsPolicy",
                  p => p.SetIsOriginAllowed((host) => true)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials()));

//configure database connection
builder.Services.ConfigureDatabaseConnection(Configuration);
builder.Services.ConfigureAppSettingsBinding(Configuration);
builder.Services.RegisterAndConfigureSwaggerAuthorizationOptions();
builder.Services.ConfigureIdentityProvider(Configuration);
builder.Services.ConfigureAuthentication(Configuration);
builder.Services.ConfigureRedisCache(Configuration);
builder.Services.RegisterServices();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var sp = app.Services.CreateScope())
{
    sp.ServiceProvider.GetService<IdentityDatabaseContext>()?.Database.Migrate();
    sp.ServiceProvider.GetService<NudjrDatabaseContext>()?.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.ConfigureMiddleWares();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
