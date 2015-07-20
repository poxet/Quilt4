using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailBusiness _emailBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;


        public ManageController(IAccountRepository accountRepository, IEmailBusiness emailBusiness, IInitiativeBusiness initiativeBusiness)
        {
            _accountRepository = accountRepository;
            _emailBusiness = emailBusiness;
            _initiativeBusiness = initiativeBusiness;
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            //Make two first users admin
            if (_accountRepository.GetUsers().Count() <= 2 && !User.IsInRole("Admin"))
            {
                var user = _accountRepository.GetUsers().Single(x => x.Email == _accountRepository.FindById(User.Identity.GetUserId()).Email);
                _accountRepository.AssignRole(user.UserId, "Admin");
            }

            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : string.Empty;

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                UserName = _accountRepository.FindById(User.Identity.GetUserId()).UserName,
                Email = _accountRepository.FindById(User.Identity.GetUserId()).Email,
                PhoneNumber = await _accountRepository.GetPhoneNumberAsync(User.Identity.GetUserId()),
                TwoFactor = await _accountRepository.GetTwoFactorEnabledAsync(User.Identity.GetUserId()),
                Logins = await _accountRepository.GetLoginsAsync(User.Identity.GetUserId()),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId())
            };
            return View(model);
        }

        public ActionResult EditEnvironmentColors()
        {
            //TODO: If provided is not the userName, but the userId, find the userId by the userName.
            var environmentColors = _initiativeBusiness.GetEnvironmentColors(User.Identity.GetUserId(), _accountRepository.FindById(User.Identity.GetUserId()).UserName);

            return View(environmentColors);
        }

        [HttpPost]
        public ActionResult EditEnvironmentColors(FormCollection collection)
        {
            var colors = new Dictionary<string, string>();

            for (var i = 0; i < collection.Count; i++)
            {
                var key = collection.GetKey(i);
                var value = collection.ToValueProvider().GetValue(key);
                colors.Add(key, value.AttemptedValue.Replace("#", ""));
            }

            _initiativeBusiness.UpdateEnvironmentColors(User.Identity.GetUserId(), colors);


            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/RemoveLogin
        public ActionResult RemoveLogin()
        {
            var linkedAccounts = _accountRepository.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;

            //TODO: Use IAccountBusiness for this...
            throw new NotImplementedException();
            
            //return System.Web.UI.WebControls.View(linkedAccounts);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await _accountRepository.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await _accountRepository.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await _accountRepository.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (_accountRepository.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await _accountRepository.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await _accountRepository.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await _accountRepository.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await _accountRepository.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await _accountRepository.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await _accountRepository.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountRepository.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await _accountRepository.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // GET: /Manage/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await _accountRepository.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await _accountRepository.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountRepository.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await _accountRepository.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await _accountRepository.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInAsync(user, isPersistent: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            var user = await _accountRepository.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await _accountRepository.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }

            var result = await _accountRepository.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //TODO: Use IAccountBusiness for this...

        private async Task SignInAsync(IApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(_accountRepository));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = _accountRepository.FindById(User.Identity.GetUserId());
            return user.HasLocalAccount;
            //if (user != null)
            //{
            //    return user.PasswordHash != null;
            //}
            //return false;
        }

        //private bool HasPhoneNumber()
        //{
        //    var user = UserManager.FindById(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        return user.PhoneNumber != null;
        //    }
        //    return false;
        //}

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion

        public ActionResult ChangeUsername()
        {
            var model = new ChangeUsernameModel()
            {
                NewUsername = _accountRepository.FindById(User.Identity.GetUserId()).UserName,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeUsername(ChangeUsernameModel model)
        {
            var username = _accountRepository.FindById(User.Identity.GetUserId()).UserName;

            var userAsync = await _accountRepository.FindAsync(username, model.Password);
            
            if (userAsync == null)
            {
                ViewBag.WrongPassword = "Incorrect password!";
                return View();
            }

            var result = await _accountRepository.UpdateUsernameAsync(userAsync.Id, model.NewUsername);
            if (result.Succeeded)
            {
                await _accountRepository.UpdateSecurityStampAsync(userAsync.Id);
                userAsync = await _accountRepository.FindAsync(model.NewUsername, model.Password);
                AuthenticationManager.SignOut();
                await _accountRepository.PasswordSignInAsync(userAsync.UserName, model.Password, false, false);
            }

            return Redirect("Index");
        }

        public ActionResult ChangeEmail()
        {
            var model = new ChangeEmailModel()
            {
                NewEmail = _accountRepository.FindById(User.Identity.GetUserId()).Email,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeEmail(ChangeEmailModel model)
        {
            var username = _accountRepository.FindById(User.Identity.GetUserId()).UserName;

            var userAsync = await _accountRepository.FindAsync(username, model.Password);

            if (userAsync == null)
            {
                ViewBag.WrongPassword = "Incorrect password!";
                return View();
            }

            var result = await _accountRepository.UpdateEmailAsync(userAsync.Id, model.NewEmail);

            if (result.Succeeded)
            {
                await _accountRepository.UpdateSecurityStampAsync(userAsync.Id);
                var token = await _accountRepository.GenerateEmailConfirmationTokenAsync(userAsync.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userAsync.Id, code = token }, protocol: Request.Url.Scheme);
                _emailBusiness.SendEmail(new List<string>() { model.NewEmail }, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                //update session cookie
                userAsync = await _accountRepository.FindAsync(username, model.Password);
                AuthenticationManager.SignOut();
                await _accountRepository.PasswordSignInAsync(userAsync.UserName, model.Password, false, false);
            }

            return Redirect("Index");
        }
    }
}