﻿using Microsoft.AspNetCore.Identity;
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
        //private IUserValidator<ApplicationUser> userValidator;
        //private IPasswordValidator<ApplicationUser> passwordValidator;
        //private IPasswordHasher<ApplicationUser> passwordHasher;

        public AdminController(ILoggerFactory loggerFactory
            , IConfiguration configuration
            , UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager
            , SignInManager<ApplicationUser> signInManager
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
            if (user != null)
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
        public async Task<RoleEditModel> RoleWithUsers([FromBody] string roleId)
        {
            RoleEditModel result = new RoleEditModel();
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

                result = new RoleEditModel
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
        public async Task<List<RoleModificationResult>> UpdateUsersInRole([FromBody] RoleModificationModel model)
        {
            List<RoleModificationResult> resultError = new List<RoleModificationResult>();
            IdentityResult result;
            //IdsToAdd из UI
            foreach (string userId in model.IdsToAdd ?? new string[] { })
            {
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    result = await _userManager.AddToRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                    {
                        resultError.Add(AddErrorsFromResult(result, model.RoleId, userId));
                    }
                }
            }
            //IdsToDelete из UI
            foreach (string userId in model.IdsToDelete ?? new string[] { })
            {
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                    {
                        resultError.Add(AddErrorsFromResult(result, model.RoleId, userId));
                    }
                }
            }
            if (resultError.Count == 0)
            {
                resultError.Add(new RoleModificationResult { Successful = SuccessfulEnum.Successful });
            }
            return resultError;
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