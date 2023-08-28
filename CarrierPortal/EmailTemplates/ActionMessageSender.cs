using CarrierPortal.Models;
using CarrierPortal.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Plugins;

namespace CarrierPortal.EmailTemplates
{
    public class ActionMessageSender
    {
        private IEmailService _emailSerivice;
        private UserManager<ApplicationUser> _userManager;

        public ActionMessageSender(IEmailService emailService,UserManager<ApplicationUser> userManager) {

            _emailSerivice = emailService;
            _userManager = userManager;
        }

        //public async Task<bool>  SendActionMessage(string userId, string URL) {

            

        //    //string emailBody = GetActionEmailBody(userName, URL);
        //    //var isSended =  await emailService.SendEmailAsync(user.Email, "Email Confirmation", emailBody);

        //    //return true;

        //}


        private string GetActionEmailBody(string userName, string URL)
        {
            var emailTemplate = System.IO.File.ReadAllText("/Action.html");
            emailTemplate = emailTemplate.Replace("{UserName}", userName);
            emailTemplate = emailTemplate.Replace("{URL}", URL);
            return emailTemplate;
        }


    }
}
