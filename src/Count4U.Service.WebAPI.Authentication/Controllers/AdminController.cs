using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
 using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Count4U.Service.Common.Filter.ActionFilterFactory;
using Count4U.Service.Core.Server.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Monitor.Service.Model;
using Monitor.Service.Urls;
using System.Collections.Generic;
using Monitor.Service.Shared;

namespace Count4U.Service.WebAPI.Authentication.Controllers
{
 //   [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ServiceFilter(typeof(ControllerTraceServiceFilter))]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountsController> _logger;
        private readonly IEmailSender _emailSender;
        //private IUserValidator<ApplicationUser> userValidator;
        //private IPasswordValidator<ApplicationUser> passwordValidator;
        //private IPasswordHasher<ApplicationUser> passwordHasher;

        public AdminController(ILoggerFactory loggerFactory
            , IConfiguration configuration
            , UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager
            , SignInManager<ApplicationUser> signInManager
            , IEmailSender emailSender
            //, IUserValidator<ApplicationUser> userValid
            //, IPasswordValidator<ApplicationUser> passValid
            //, IPasswordHasher<ApplicationUser> passwordHash
            )
        {
            this._logger = loggerFactory.CreateLogger<AccountsController>();
            this._configuration = configuration ??
                              throw new ArgumentNullException(nameof(configuration));
            this._userManager = userManager ??
                              throw new ArgumentNullException(nameof(userManager));
            this._roleManager = roleManager ??
                            throw new ArgumentNullException(nameof(roleManager));
            this._signInManager = signInManager ??
                            throw new ArgumentNullException(nameof(signInManager));
			this._emailSender = emailSender ??
							  throw new ArgumentNullException(nameof(emailSender));
        }

        //public ViewResult Index() => View(_userManager.Users);



        [HttpPost(WebApiAuthenticationAdmin.Delete)]
        public async Task<DeleteResult> Delete([FromBody] DeleteModel deleteModel)
        {
            if (deleteModel == null)
            {
                return new DeleteResult { Successful = SuccessfulEnum.NotSuccessful, Error = " DeleteModel is null " };
            }

            if (string.IsNullOrWhiteSpace(deleteModel.ApplicationUserID))
            {
                return new DeleteResult { Successful = SuccessfulEnum.NotSuccessful, Error = "User ID is empty " };
            }

            ApplicationUser user = await _userManager.FindByIdAsync(deleteModel.ApplicationUserID);
            if (user == null)
            {
                return new DeleteResult { Successful = SuccessfulEnum.NotSuccessful, Error = "Can't get user from db " };
            }

            IdentityResult result = await _userManager.DeleteAsync(user);
            if (result.Succeeded == false)
            {
                var errors = result.Errors.Select(x => x.Description);
                var error = string.Join(" .", errors);
                return new DeleteResult { Successful = SuccessfulEnum.NotSuccessful, Error = error };
            }

            return new DeleteResult { Successful = SuccessfulEnum.Successful };
        }

        [HttpPost(WebApiAuthenticationAdmin.PostChangePassword)]
        public async Task<ChangePasswordResult> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            if (changePasswordModel == null)
            {
                return new ChangePasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error = "changePasswordModel is null " };
            }
            ApplicationUser user = null;
            if (string.IsNullOrWhiteSpace(changePasswordModel.ApplicationUserID) == false)
            {
                user = await _userManager.FindByIdAsync(changePasswordModel.ApplicationUserID);           //try by ID first
            }

            if (user == null)
            {
                if (string.IsNullOrWhiteSpace(changePasswordModel.Email) == false)
                {
                    user = await _userManager.FindByEmailAsync(changePasswordModel.Email);
                }
            }

            if (user == null)
            {
                return new ChangePasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error = "User not find " };
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
            if (result.Succeeded == false)
            {
                var errors = result.Errors.Select(x => x.Description);
                var error = string.Join(" .", errors);
                return new ChangePasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error = error };
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            return new ChangePasswordResult { Successful = SuccessfulEnum.Successful };

        }

       	[HttpPost(WebApiAuthenticationAdmin.ForgotPassword)]
		public async Task<ForgotPasswordResult> ForgotPassword([FromBody]ForgotPasswordModel forgotPasswordModel)
		{
            if (forgotPasswordModel == null)
            {
               	return new ForgotPasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error ="can't find user by email" };
            }

            ApplicationUser user = null;
            if (string.IsNullOrWhiteSpace(forgotPasswordModel.ApplicationUserID) == false)
            {
                user = await _userManager.FindByIdAsync(forgotPasswordModel.ApplicationUserID);           //try by ID first
            }

            if (user == null)
            {
                if (string.IsNullOrWhiteSpace(forgotPasswordModel.Email) == false)
                {
                    user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                }
            }

            if (user == null)
            {
                	return new ForgotPasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error ="can't find user" };
            }
            try
            {
                if (string.IsNullOrWhiteSpace(forgotPasswordModel.NewPassword) == true)
                    forgotPasswordModel.NewPassword = RandomGenerator.RandomString(6, true);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, forgotPasswordModel.NewPassword);
                if (result.Succeeded)
                {
                     string content = $"Hello {user.Email}, <br> Your password was changed to new : <br> <p> {user.Email} </p><p> {forgotPasswordModel.NewPassword} </p>";
           			var message = new EmailMessage(new string[] {user.Email }, "Password changed", content, null);
					await _emailSender.SendEmailAsync(message);
					return new ForgotPasswordResult { Successful = SuccessfulEnum.Successful, Token = token, Email = user.Email };
                }
                else
                {
                    string errorResult = "";
                    foreach (var error in result.Errors)
                    {
                       errorResult +=  error.Description + "; " ;
                    }
                    return new ForgotPasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error = errorResult};
                }
            }
            catch (Exception exc)
            {
                return new ForgotPasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error = exc.Message };
            }

		}
        
        // ===================== User ========================
        [HttpGet(WebApiAuthenticationAdmin.GetUsers)]
        public async Task<List<UserViewModel>> GetUsers()
        {

            List<UserViewModel> result = new List<UserViewModel>();
            try
            {
                foreach (ApplicationUser user in this._userManager.Users)
                {
                    if (user != null)
                    {
                        result.Add(new UserViewModel { UserID = user.Id, Email = user.Email });
                    }
                }
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }

         [HttpPost(WebApiAuthenticationAdmin.GetUserWithPassword)]
        public async Task<UserWithPasswordModel> GetUserWithPassword([FromBody] UserWithPasswordModel userWithPasswordModel)
        {

             if (userWithPasswordModel == null)
            {
                return userWithPasswordModel;
            }
            ApplicationUser user = null;
            if (string.IsNullOrWhiteSpace(userWithPasswordModel.ApplicationUserID) == false)
            {
                user = await _userManager.FindByIdAsync(userWithPasswordModel.ApplicationUserID);           //try by ID first
            }

            if (user == null)
            {
                if (string.IsNullOrWhiteSpace(userWithPasswordModel.Email) == false)
                {
                    user = await _userManager.FindByEmailAsync(userWithPasswordModel.Email);
                }
            }

            if (user == null)
            {
                return userWithPasswordModel;
            }

            return userWithPasswordModel;
        }

        // ==================  Role   ========================
        [HttpGet(WebApiAuthenticationAdmin.GetRoles)]
        public async Task<List<RoleModel>> GetRoles()
        {
            List<RoleModel> result = new List<RoleModel>();
            try
            {
                foreach (IdentityRole role in _roleManager.Roles)
                {
                    if (role != null)
                    {
                        List<UserViewModel> members = new List<UserViewModel>();
                        foreach (ApplicationUser user in _userManager.Users)
                        {
                            if (user != null)
                            {
                                if (await _userManager.IsInRoleAsync(user, role.Name))
                                {
                                    members.Add(new UserViewModel { UserID = user.Id, Email = user.Email });
                                }
                            }
                        }
                        result.Add(new RoleModel { RoleID = role.Id, RoleName = role.Name, Members = members });
                    }
                }
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }



        //Заполняем форму редактирования 
        [HttpPost(WebApiAuthenticationAdmin.RoleWithUsers)]
        public async Task<RoleModel> RoleWithUsers([FromBody] string roleId)
        {
            RoleModel result = new RoleModel();
            try
            {
                IdentityRole role = await _roleManager.FindByIdAsync(roleId);
                List<UserViewModel> members = new List<UserViewModel>();
                List<UserViewModel> nonMembers = new List<UserViewModel>();
                foreach (ApplicationUser user in _userManager.Users)
                {
                    var list = await _userManager.IsInRoleAsync(user, role.Name)
                        ? members : nonMembers;
                    list.Add(new UserViewModel { UserID = user.Id, Email = user.Email });
                }

                result = new RoleModel
                {
                    RoleID = roleId,
                    RoleName = role.Name,
                    Members = members,
                    NonMembers = nonMembers
                };
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }


        [HttpPost(WebApiAuthenticationAdmin.UpdateUsersInRole)]
        public async Task<RoleResult> UpdateUsersInRole([FromBody] RoleModel roleModel)
        {
            List<RoleModificationResult>  resultError = new List<RoleModificationResult>();
        	if (roleModel == null)
			{
				return new RoleResult { Successful = SuccessfulEnum.NotSuccessful, Error = "RoleModel is null " };
			}

            IdentityResult result;
            //IdsToAdd из UI
            //foreach (string userId in model.IdsToAdd ?? new string[] { })
            foreach (UserViewModel userMember in roleModel.NonMembers)
            {
                if (userMember.ToAdd == true)
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userMember.UserID);
                    if (user != null)
                    {
                        result = await _userManager.AddToRoleAsync(user, roleModel.RoleName);
                        if (!result.Succeeded)
                        {
                            resultError.Add(AddErrorsFromResult(result, roleModel.RoleID, userMember.UserID));
                        }
                    }
                }
            }
            //IdsToDelete из UI
            // foreach (string userId in model.IdsToDelete ?? new string[] { })
            foreach (UserViewModel userMember in roleModel.Members)
            {
                if (userMember.ToDelete == true)
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userMember.UserID);
                    if (user != null)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, roleModel.RoleName);
                        if (!result.Succeeded)
                        {
                            resultError.Add(AddErrorsFromResult(result, roleModel.RoleID, userMember.UserID));
                        }
                    }
                }
            }


            if (resultError.Count == 0)
            {
                return new RoleResult { Successful = SuccessfulEnum.Successful };
            }
            else
            {
                return new RoleResult { Successful = SuccessfulEnum.Successful };
            }
        }

        [NonAction]
        private RoleModificationResult AddErrorsFromResult(IdentityResult result, string roleId, string userId)
        {
            var errors = result.Errors.Select(x => x.Description);
            var error = string.Join(" .", errors);
            return new RoleModificationResult { Successful = SuccessfulEnum.NotSuccessful, Error = error, RoleID = roleId, ApplicationUserID = userId };

        }
    }
}
