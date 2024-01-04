﻿using CarrierPortal.Models;
using CarrierPortal.Services.EmailServices;
using CarrierPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarrierPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IEmailService emailService;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger, IEmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await userManager.GetUserAsync(User);

            var userHasPassword = await userManager.HasPasswordAsync(user);

            if (userHasPassword)
            {
                return RedirectToAction("ChangePassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                var result = await userManager.AddPasswordAsync(user, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);

                return View("AddPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);

            var userHasPassword = await userManager.HasPasswordAsync(user);

            if (!userHasPassword)
            {
                return RedirectToAction("AddPassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if (await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }

                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);



                    var emailBody = GetForgotPasswordEmailBody(user.UserName, passwordResetLink);
                    await emailService.SendEmailAsync(user.Email, "Password Reset", emailBody.ToString());




                   // logger.Log(LogLevel.Warning, passwordResetLink);
                    //await  new EmailService().SendEmailAsync(model.Email, "forget password your mail ", passwordResetLink);

                    return View("ForgotPasswordConfirmation");
                }

                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        private string GetForgotPasswordEmailBody(string userName, string resetLink)
        {
            var emailTemplate = System.IO.File.ReadAllText("EmailTemplates/ForgotPassword.html");
            emailTemplate = emailTemplate.Replace("{UserName}", userName);
            emailTemplate = emailTemplate.Replace("{ResetLink}", resetLink);
            return emailTemplate;
        }




        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City,
                    PhoneNumber = model.Phone
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {


                    var rolesToAdd = new List<string> { "Blog", "MentorShip", "QnA", "Job" };
                    await userManager.AddToRolesAsync(user, rolesToAdd);

                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                            new { userId = user.Id, token = token }, Request.Scheme);


                    var emailBody = GetConfirmationEmailBody(user.UserName, confirmationLink);
                    emailService.SendEmailAsync(user.Email, "Email Confirmation", emailBody);


                    // var to = new List<string>();
                    //to.Add(model.Email);
                    //await new EmailService().SendEmailAsync(model.Email, "Confirm your mail ", confirmationLink);

                    logger.Log(LogLevel.Warning, confirmationLink);

                    //if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    //{
                    //    return RedirectToAction("ListUsers", "Administration");
                    //}

                    ViewBag.ErrorTitle = "  Registration successful";
                    ViewBag.ErrorMessage = "  Before you can Login, please confirm your " +
                        "email, by clicking on the confirmation link we have emailed you";
                    return View("Error");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }




        private string GetConfirmationEmailBody(string userName, string confirmationLink)
        {
            var emailTemplate = System.IO.File.ReadAllText("EmailTemplates/ConfirmationEmail.html");
            emailTemplate = emailTemplate.Replace("{UserName}", userName);
            emailTemplate = emailTemplate.Replace("{ConfirmationLink}", confirmationLink);
            return emailTemplate;
        }




        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {

                return RedirectToAction("Index", "Home");
            }

            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

          

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed &&
                                    (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email forget not confirmed yet");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,
                                        model.RememberMe, true);

                if (result.Succeeded)
                {
                    
                        return RedirectToAction("index", "home");
                    
                }

                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }



        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        //{
        //   model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        //    if (ModelState.IsValid)
        //    {
        //        var user = await userManager.FindByEmailAsync(model.Email);

        //        if (user != null && !user.EmailConfirmed &&
        //                            (await userManager.CheckPasswordAsync(user, model.Password)))
        //        {
        //            ModelState.AddModelError(string.Empty, "Email not confirmed yet");
        //            return View(model);
        //        }

        //        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,
        //                                model.RememberMe, true);

        //        if (result.Succeeded)
        //        {
        //            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //            {
        //                return Redirect(returnUrl);
        //            }
        //            else
        //            {
        //                return RedirectToAction("index", "home");
        //            }
        //        }

        //        if (result.IsLockedOut)
        //        {
        //            return View("AccountLocked");
        //        }

        //        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
        //    }

        //    return View(model);
        //}

        //    [AllowAnonymous]
        //    [HttpPost]
        //    public IActionResult ExternalLogin(string provider, string returnUrl)
        //    {
        //        var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
        //                                new { ReturnUrl = returnUrl });

        //        var properties =
        //            signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        //        return new ChallengeResult(provider, properties);
        //    }

        //    [AllowAnonymous]
        //    public async Task<IActionResult>
        //        ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        //    {
        //        returnUrl = returnUrl ?? Url.Content("~/");

        //        LoginViewModel loginViewModel = new LoginViewModel
        //        {
        //            ReturnUrl = returnUrl,
        //            ExternalLogins =
        //            (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
        //        };

        //        if (remoteError != null)
        //        {
        //            ModelState.AddModelError(string.Empty,
        //                $"Error from external provider: {remoteError}");

        //            return View("Login", loginViewModel);
        //        }

        //        var info = await signInManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            ModelState.AddModelError(string.Empty,
        //                "Error loading external login information.");

        //            return View("Login", loginViewModel);
        //        }

        //        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //        ApplicationUser user = null;

        //        if (email != null)
        //        {
        //            user = await userManager.FindByEmailAsync(email);

        //            if (user != null && !user.EmailConfirmed)
        //            {
        //                ModelState.AddModelError(string.Empty, "Email not confirmed yet");
        //                return View("Login", loginViewModel);
        //            }
        //        }

        //        var signInResult = await signInManager.ExternalLoginSignInAsync(
        //                                    info.LoginProvider, info.ProviderKey,
        //                                    isPersistent: false, bypassTwoFactor: true);

        //        if (signInResult.Succeeded)
        //        {
        //            return LocalRedirect(returnUrl);
        //        }
        //        else
        //        {
        //            if (email != null)
        //            {
        //                if (user == null)
        //                {
        //                    user = new ApplicationUser
        //                    {
        //                        UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
        //                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
        //                    };

        //                    await userManager.CreateAsync(user);

        //                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        //                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
        //                                    new { userId = user.Id, token = token }, Request.Scheme);

        //                    logger.Log(LogLevel.Warning, confirmationLink);

        //                    ViewBag.ErrorTitle = "Registration successful";
        //                    ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
        //                        "email, by clicking on the confirmation link we have emailed you";
        //                    return View("Error");
        //                }

        //                await userManager.AddLoginAsync(user, info);
        //                await signInManager.SignInAsync(user, isPersistent: false);

        //                return LocalRedirect(returnUrl);
        //            }

        //            ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
        //            ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

        //            return View("Error");
        //        }
        //    }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }



    }
}
