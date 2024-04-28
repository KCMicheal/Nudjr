using AutoMapper;
using Nudjr_AppCore.Services.IdentityServices.Interfaces;
using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_AppCore.Services.Shared.Services;
using Nudjr_Domain.Entities;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.ConfigModels;
using Nudjr_Domain.Models.ExceptionModels;
using Nudjr_Domain.Models.IdentityModels;
using Nudjr_Domain.Models.ServiceModels;
using Nudjr_Domain.Models.ServiceModels.NovuModels;
using Nudjr_Domain.Models.ViewModels;
using Nudjr_Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Nudjr_AppCore.Services.IdentityServices.Services
{
    public class UserAccountService : BaseEntityService, IUserAccountService
    {
        private readonly IServiceFactory ServiceFactory;
        private readonly ILoggerManager LoggerManager;
        private readonly INovuNotificationService NotificationService;
        private readonly NovuConfig _novuConfig;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly int PIN_LENGTH = 6;
        private const string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        public UserAccountService(IServiceFactory serviceFactory, ILoggerManager loggerManager, UserManager<ApplicationUser> userManager,
            INovuNotificationService novuNotificationService, IOptionsSnapshot<NovuConfig> novuConfig, IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            ServiceFactory = serviceFactory;
            LoggerManager = loggerManager;
            UserManager = userManager;
            NotificationService = novuNotificationService;
            _novuConfig = novuConfig.Value;
        }
        public async Task<bool> AssignUserToRole(ApplicationUser appUser, Guid assigneeUserId, PersonType newRole)
        {
            IUserRoleService? userRoleService = ServiceFactory.GetService(typeof(IUserRoleService)) as IUserRoleService;

            USER assigneeUserProfile = await _unitOfWork.GetRepository<USER>()
                .FirstOrDefaultAsync(x => x.Id == assigneeUserId && x.EntityStatus == EntityStatus.ACTIVE,
                disableTracking: false);

            if (assigneeUserProfile is null)
                throw new NotFoundException("Assignee Profile Not Found");

            bool isSuccess = await userRoleService.AssignUserToRole(appUser, assigneeUserProfile.IdentityUserId, newRole);
            if (isSuccess)
            {
                //Update user's profile if user is assigned to a higher role
                if (newRole > assigneeUserProfile.PersonType)
                {
                    assigneeUserProfile.PersonType = newRole;
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            return isSuccess;
        }

        public async Task<PersonType> CheckUserRole(ApplicationUser user, string loggedInAs, bool isAdminLogin = false)
        {
            IUserRoleService userRoleService = ServiceFactory.GetService(typeof(IUserRoleService)) as IUserRoleService;
            PersonType? userRole = null;
            if (!isAdminLogin)
            {
                bool isPersonType = Enum.TryParse(loggedInAs, out PersonType personType);
                if (!isPersonType)
                    throw new InvalidOperationException("Invalid Login");

                if ((personType != PersonType.User && personType != PersonType.Support))
                    throw new InvalidOperationException("Access Denied");

                bool isUserInRole = await userRoleService.IsUserInRole(user, personType);
                if (!isUserInRole)
                    throw new InvalidOperationException("Access Denied");

                userRole = personType;
            }
            else
            {
                IList<PersonType> userRoles = await userRoleService.GetUserRoles(user);
                if (userRoles.Contains(PersonType.Support))
                {
                    userRole = PersonType.Support;
                }
                if (userRoles.Contains(PersonType.Admin))
                {
                    userRole = PersonType.Admin;
                }
                if (userRoles.Contains(PersonType.SuperAdmin))
                {
                    userRole = PersonType.SuperAdmin;
                }
                if (!userRole.HasValue)
                {
                    throw new InvalidOperationException("Access Denied");
                }
            }

            return userRole.Value;
        }

        public async Task<USER> CreateUserAccount(UserSignUpDto model, string userName, string pin)
        {
            IIdentityUserService? identityUserService = ServiceFactory.GetService(typeof(IIdentityUserService)) as IIdentityUserService;

            if (model is null)
                throw new InvalidOperationException("User Details Can't Be Empty");

            if (model.PersonType == PersonType.Admin || model.PersonType == PersonType.SuperAdmin)
                throw new InvalidOperationException("User Can't Be Signed Up As Admin Or Super Admin");

            USER tribeUser = await _unitOfWork.GetRepository<USER>().FirstOrDefaultAsync(x => x.EmailAddress == model.EmailAddress);
            if (tribeUser is not null)
                throw new InvalidOperationException("User Already Exists!");

            ApplicationUser applicationUser = new ApplicationUser()
            {
                Email = model.EmailAddress,
                UserName = userName,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true
            };

            if (pin.Length != PIN_LENGTH)
                throw new InvalidOperationException($"Password length must be {PIN_LENGTH} numbers");

            IdentityResult result = await identityUserService.CreateIdentityUser(applicationUser, model.PersonType, pin);

            if (result.Succeeded)
            {
                tribeUser = new USER()
                {
                    EntityStatus = EntityStatus.ACTIVE,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    IdentityUserId = applicationUser.Id,
                    Username = model.Username,
                    DateOfBirth = model.DateOfBirth,
                    Nationality = model.Nationality,
                    Gender = model.Gender,
                    PersonType = model.PersonType
                };

                tribeUser = _unitOfWork.GetRepository<USER>().Add(tribeUser);

                int recordCount = await _unitOfWork.SaveChangesAsync();
                if (recordCount > 0)
                {
                    NovuOperationModel subscriber = await NotificationService.CreateSubscriber(tribeUser.Id);

                    await NotificationService.SendNotificationToASubscriber(subscriber.Result.ToString(), _novuConfig.OnboardingMessage,
                        new WelcomeMessage { FirstName = tribeUser.FirstName, LastName = tribeUser.LastName });

                    return tribeUser;
                }
                else
                {
                    throw new AppException("Could Not Complete Registration Process!");
                }
            }
            else
            {
                LoggerManager.LogError($"Could not create identity user during account creation  [CreateUserAccount] | {JsonConvert.SerializeObject(result)}");
                throw new InvalidOperationException("Could Not Complete Signup Process");
            }

        }

        public async Task<USER> CreateAdminAccount(UserSignUpDto model)
        {
            IIdentityUserService? identityUserService = ServiceFactory.GetService(typeof(IIdentityUserService)) as IIdentityUserService;

            if (model is null)
                throw new InvalidOperationException("User Details Can't Be Empty");

            if (model.PersonType == PersonType.Support || model.PersonType == PersonType.User)
                throw new InvalidOperationException("User Can't Be Signed Up As Member Or Support");

            USER tribeUser = await _unitOfWork.GetRepository<USER>().FirstOrDefaultAsync(x => x.EmailAddress == model.EmailAddress);
            if (tribeUser is not null)
                throw new InvalidOperationException("User Already Exists!");

            ApplicationUser applicationUser = new ApplicationUser()
            {
                Email = model.EmailAddress,
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,
            };

            IdentityResult result = await identityUserService.CreateIdentityUser(applicationUser, model.PersonType, model.Password);

            if (result.Succeeded)
            {
                tribeUser = new USER()
                {
                    EntityStatus = EntityStatus.ACTIVE,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    IdentityUserId = applicationUser.Id,
                    Username = model.Username,
                    DateOfBirth = model.DateOfBirth,
                    Nationality = model.Nationality,
                    Gender = model.Gender,
                    PersonType = model.PersonType
                };

                tribeUser = _unitOfWork.GetRepository<USER>().Add(tribeUser);

                int recordCount = await _unitOfWork.SaveChangesAsync();
                if (recordCount > 0)
                {
                    NovuOperationModel subscriber = await NotificationService.CreateSubscriber(tribeUser.Id);

                    await NotificationService.SendNotificationToASubscriber(subscriber.Result.ToString(), _novuConfig.OnboardingMessage,
                        new WelcomeMessage { FirstName = tribeUser.FirstName, LastName = tribeUser.LastName });

                    return tribeUser;
                }
                else
                {
                    throw new AppException("Could Not Complete Registration Process!");
                }
            }
            else
            {
                LoggerManager.LogError($"Could not create identity user during account creation  [CreateUserAccount] | {JsonConvert.SerializeObject(result)}");
                throw new InvalidOperationException("Could Not Complete Signup Process");
            }

        }

        public async Task<USER> GetUser(ClaimsPrincipal claimsPrincipal)
        {
            string? identityUserId = UserManager.GetUserId(claimsPrincipal);
            USER user = await _unitOfWork.GetRepository<USER>()
                .SingleOrDefaultAsync(p => p.IdentityUserId == identityUserId);
            if (user == null)
                throw new NotFoundException("User Not Found");

            return user;
        }

        public async Task<Guid> GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            string? identityUserId = UserManager.GetUserId(claimsPrincipal);
            USER user = await _unitOfWork.GetRepository<USER>()
                .SingleOrDefaultAsync(p => p.IdentityUserId == identityUserId);
            if (user == null)
                throw new NotFoundException("User Not Found");

            return user.Id;
        }

        public async Task<USER> GetUserByEmail(string emailAddress)
        {
            USER user = await _unitOfWork.GetRepository<USER>()
                .SingleOrDefaultAsync(p => p.EmailAddress == emailAddress);
            if (user == null)
                throw new NotFoundException("User Not Found");

            return user;
        }

        public async Task<Jwt> GenerateLoginTokenAfterAccountCreation(USER user)
        {
            try
            {
                IJwtService jwtService = ServiceFactory.GetService(typeof(IJwtService)) as IJwtService;
                IIdentityUserService identityUserService = ServiceFactory.GetService(typeof(IIdentityUserService)) as IIdentityUserService;

                ApplicationUser applicationUser = await identityUserService.GetApplicationUserAsync(user.IdentityUserId);
                Jwt jwt = jwtService.GenerateJwtToken(applicationUser, user.PersonType);

                return jwt;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<JwtWithRefreshToken> UserLogin(string userNameOrEmailAddress, string password, PersonType personType)
        {
            IRefreshTokenService refreshTokenService = ServiceFactory.GetService(typeof(IRefreshTokenService)) as IRefreshTokenService;

            USER user;
            if (string.IsNullOrWhiteSpace(userNameOrEmailAddress) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Please Provide Username &/or Password");

            if (personType == PersonType.Admin || personType == PersonType.SuperAdmin)
                throw new InvalidOperationException("Sorry, Can Only Login As Member Or Support");

            string? userEmail = Regex.IsMatch(userNameOrEmailAddress.Trim(), pattern) ? userNameOrEmailAddress : null;

            if (userEmail is not null)
            {
                user = await GetUserByEmail(userEmail);
            }
            else
            {
                user = await GetUserByUserName(userNameOrEmailAddress);
            }

            LoginRequest loginModel = new LoginRequest()
            {
                Email = user.EmailAddress,
                Password = password,
                LoggedInAs = personType.ToString(),
            };

            return await Login(loginModel);
        }

        public async Task<Jwt> AdminLogin(string userNameOrEmailAddress, string password)
        {
            USER user;
            string? userEmail = Regex.IsMatch(userNameOrEmailAddress.Trim(), pattern) ? userNameOrEmailAddress : null;

            if (userEmail is not null)
            {
                user = await GetUserByEmail(userEmail);
            }
            else
            {
                user = await GetUserByUserName(userNameOrEmailAddress);
            }
            LoginRequest loginRequestModel = new LoginRequest
            {
                Email = user.EmailAddress,
                Password = password,
                LoggedInAs = user.PersonType.ToString(),
            };
            return await Login(loginRequestModel, true);
        }

        public async Task<JwtWithRefreshToken> RefreshToken(string token, string refreshToken)
        {
            IRefreshTokenService refreshTokenService = ServiceFactory.GetService(typeof(IRefreshTokenService)) as IRefreshTokenService;
            JwtWithRefreshToken jwtWithRefreshToken = await refreshTokenService.Refresh(token, refreshToken);
            return jwtWithRefreshToken;
        }

        private async Task<JwtWithRefreshToken> Login(LoginRequest model, bool isAdminLogin = false)
        {
            JwtWithRefreshToken jwt = null;
            IJwtService jwtService = ServiceFactory.GetService(typeof(IJwtService)) as IJwtService;
            IIdentityUserService identityService = ServiceFactory.GetService(typeof(IIdentityUserService)) as IIdentityUserService;

            ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                throw new InvalidOperationException("Incorrect email or password");
            }


            //Validate user password
            bool isPasswordCorrect = await ValidateUserPassword(user, model.Password);


            if (!isPasswordCorrect)
            {

                //Check if user has exceeded the maximum failed password attempts within configured duration
                bool isLockedOut = await UserManager.IsLockedOutAsync(user);
                if (isLockedOut)
                    throw new InvalidOperationException("Too Many Failed Login Attempts. Please Try Again After Some Minutes");


                //Increment The No Of Failed Login Attempts
                await UserManager.AccessFailedAsync(user);

                throw new InvalidOperationException("Incorrect Password");
            }


            PersonType loggedInAs = await CheckUserRole(user, model.LoggedInAs, isAdminLogin);


            jwt = await jwtService.GenerateJWtWithRefreshTokenAsync(user, loggedInAs);

            await UserManager.ResetAccessFailedCountAsync(user);

            await UserManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.UtcNow));

            // change the security stamp and logout from other devices
            await UserManager.UpdateSecurityStampAsync(user);

            return jwt;

        }


        private async Task<bool> ValidateUserPassword(ApplicationUser appUser, string password)
        {
            bool isCorrectPassword = await UserManager.CheckPasswordAsync(appUser, password);
            return isCorrectPassword;
        }

        private async Task<USER> GetUserByUserName(string userName)
        {
            USER user = await _unitOfWork.GetRepository<USER>()
                .SingleOrDefaultAsync(p => p.Username == userName);
            if (user == null)
                throw new NotFoundException("User Not Found");

            return user;
        }
    }
}
