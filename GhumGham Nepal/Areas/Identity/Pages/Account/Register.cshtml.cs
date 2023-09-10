// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using GhumGhamNepal.Core.Services.EmailService;
using GhumGhamNepal.Core.Enums;
using GhumGhamNepal.Core.Models.Identity;
using GhumGham_Nepal.Repository;
using GhumGhamNepal.Core.Models.DbEntity;
using GhumGhamNepal.Core.Services.AttachmentService;
using Humanizer;
using GhumGham_Nepal.Services;
using GhumGhamNepal.Core.Services;

namespace GhumGham_Nepal.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<GhumGhamNepalUser> _signInManager;
        private readonly UserManager<GhumGhamNepalUser> _userManager;
        private readonly IUserStore<GhumGhamNepalUser> _userStore;
        private readonly IUserEmailStore<GhumGhamNepalUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ISmtpEmailService _emailService;
        private readonly ICommonAttachmentService _commonAttachmentService;
        private readonly IRepository<CommonAttachment> _attachmentRepository;
        private readonly IHttpContextService _httpContextService;

        public RegisterModel(
            UserManager<GhumGhamNepalUser> userManager,
            IUserStore<GhumGhamNepalUser> userStore,
            SignInManager<GhumGhamNepalUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ISmtpEmailService emailService,
            IRepository<CommonAttachment> attachmentRepository,
            ICommonAttachmentService commonAttachmentService,
            IHttpContextService httpContextService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _emailService = emailService;
            _attachmentRepository = attachmentRepository;
            _commonAttachmentService = commonAttachmentService;
            _httpContextService = httpContextService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }


            //[Required]
            //[Display(Name = "UserName")]
            //public string Username { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            public IFormFile UserPhoto { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var errorList = new List<string>();
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // todo::this two should be set in transaction
                var result = await _userManager.CreateAsync(user, Input.Password);
                // save profile photo 
                var uploadPicture = await SaveUserProfilePhotoAsync(Input.UserPhoto, user.Id).ConfigureAwait(false);


                if (result.Succeeded && uploadPicture.Status)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    var body = _emailService.GetEmailTemplateContent(EmailContentFile.EmailConfirmation);
                    var subject = EmailSubject.EmailConfirmation;

                    body = body.Replace("#UserName#", user.UserName)
                        .Replace("#username#", user.UserName)
                        .Replace("#Link#", callbackUrl)
                        .Replace("#ResetURL#", callbackUrl);

                    var sendEmailResp = await _emailService.SendAsync(subject, Input.Email, body).ConfigureAwait(false);

                    //SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                if (result.Errors.Any())
                {
                    foreach (var error in result.Errors)
                    {
                        errorList.Add(error.Description);
                    }
                }
                if (!uploadPicture.Status)
                    errorList.Add(uploadPicture.Message.ToString());

                foreach (var error in errorList)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        //private bool SendEmailAsync(string email, string subject, string confirmLink)
        //{
        //    try
        //    {
        //        MailMessage message = new MailMessage();
        //        message.From = new MailAddress("ghumghamnepal001@gmail.com");
        //        message.To.Add(email);
        //        message.Subject = subject;
        //        message.IsBodyHtml = true;
        //        message.Body = confirmLink;

        //        SmtpClient smtpClient = new SmtpClient();
        //        smtpClient.Port = 587;
        //        smtpClient.Host = "smtp.gmail.com";
        //        smtpClient.EnableSsl = true;
        //        smtpClient.UseDefaultCredentials = false;
        //        smtpClient.Credentials = new NetworkCredential("ghumghamnepal001@gmail.com", "dnydbsailrhxzaaj");
        //        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        smtpClient.Send(message);
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }

        //}

        private GhumGhamNepalUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<GhumGhamNepalUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(GhumGhamNepalUser)}'. " +
                    $"Ensure that '{nameof(GhumGhamNepalUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<GhumGhamNepalUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<GhumGhamNepalUser>)_userStore;
        }

        private async Task<ServiceResult> SaveUserProfilePhotoAsync(IFormFile file, string parentRecordId)
        {
            try
            {
                List<IFormFile> picture = new();
                if (file != null)
                {
                    picture.Add(file);
                }

                var attachmentUploadResp = _commonAttachmentService.UploadCommonAttachment(picture
                   .Select(x => new FileUploadRequest(x.ReadBytes(), x.ContentType, x.FileName)).ToList());

                foreach (var item in attachmentUploadResp.Data)
                {
                    CommonAttachment attachment = new CommonAttachment();
                    attachment.ParentTableName = "AspNetUsers";
                    attachment.ParentRecordID = parentRecordId;
                    attachment.ServerFileName = item.ServerFileName;
                    attachment.UserFileName = item.UserFileName;
                    attachment.FileFormat = item.FileFormat;
                    attachment.FileType = item.FileType;
                    attachment.FileLocation = item.FileLocation;
                    attachment.Size = item.Size;
                    attachment.CreatedBy = new Guid();
                    attachment.CreatedOn = DateTime.Now;

                    await _attachmentRepository.AddAsync(attachment).ConfigureAwait(false);
                    await _attachmentRepository.CommitAsync().ConfigureAwait(false);
                }


                return ServiceResult.Success("");
            }
            catch (Exception)
            {
                return ServiceResult.Fail("Failed to save profile photo.");
            }
        }
    }
}
