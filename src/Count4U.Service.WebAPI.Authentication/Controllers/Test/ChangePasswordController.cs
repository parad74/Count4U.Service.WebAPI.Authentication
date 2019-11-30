using Count4U.Service.Core.Server.Data;
using Count4U.Service.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Count4U.Service.WebAPI.Authentication.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class ChangePasswordController : ControllerBase
    {
         private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChangePasswordController> _logger;
        private readonly IUrlHelper _urlHelper;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RavenDBIdUtil _ravenDBIdUtil;

        public ChangePasswordController(ILoggerFactory loggerFactory,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor,
            RavenDBIdUtil ravenDBIdUtil ,
            IUrlHelper urlHelper)
        {
            this._logger = loggerFactory.CreateLogger<ChangePasswordController>();
            this._configuration = configuration ??
                              throw new ArgumentNullException(nameof(configuration));
            this._userManager = userManager ??
                              throw new ArgumentNullException(nameof(userManager));
            this._signInManager = signInManager ??
                              throw new ArgumentNullException(nameof(signInManager));
            this._urlHelper = urlHelper ??
                             throw new ArgumentNullException(nameof(urlHelper));
            this._emailSender = emailSender ??
                          throw new ArgumentNullException(nameof(emailSender));
            this._httpContextAccessor = httpContextAccessor ??
                        throw new ArgumentNullException(nameof(httpContextAccessor));
            this._ravenDBIdUtil = ravenDBIdUtil ??
                      throw new ArgumentNullException(nameof(ravenDBIdUtil));

        }

        //public async Task<CommandResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        //{

        //    var user = await _mediator.Send(new CurrentUserQuery());
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load user");
        //    }
        //    if (string.IsNullOrEmpty(user.PasswordHash))
        //    {
        //        return new CommandResult("User", "User has no password set");
        //    }
        //    var changePasswordResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        //    if (!changePasswordResult.Succeeded)
        //    {
        //        // see https://github.com/aspnet/AspNetCore/blob/bfec2c14be1e65f7dd361a43950d4c848ad0cd35/src/Identity/Extensions.Core/src/IdentityErrorDescriber.cs
        //        // for diffrent error codes
        //        var keyMapping = new Dictionary<string, string>()
        //        {
        //            {"PasswordMismatch","CurrentPassword" },
        //            {"PasswordTooShort","NewPassword" },
        //            {"PasswordRequiresUniqueChars","NewPassword" },
        //            {"PasswordRequiresNonAlphanumeric","NewPassword" },
        //            {"PasswordRequiresDigit","NewPassword" },
        //            {"PasswordRequiresLower","NewPassword" },
        //            {"PasswordRequiresUpper","NewPassword" },

        //        };
        //        var formatedErrors = changePasswordResult.Errors
        //            .Select(e =>
        //            {
        //                var key = e.Code;
        //                keyMapping.TryGetValue(key, out key);
        //                return new { Key = key, e.Description };
        //            }
        //            ).ToLookup(e => e.Key, e => e.Description)
        //            .ToDictionary(l => l.Key, l => l.ToList());
        //        return new CommandResult(formatedErrors);
        //    }

        //    await _signInManager.SignInAsync(user, isPersistent: false);
        //    _logger.LogInformation("User changed their password successfully.");
        //    return CommandResult.Success();
        //}


        private async Task SendEmail(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = _urlHelper.EmailConfirmationLink(_ravenDBIdUtil.GetUrlId(user.Id), code, _httpContextAccessor.HttpContext.Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(user.Email, user.UserName, callbackUrl);
        }

    }
}
