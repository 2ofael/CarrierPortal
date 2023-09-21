using CarrierPortal.Models;
using CarrierPortal.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Plugins;
using System.Security.Policy;

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


        public async Task<bool> SendActionMessage(string userId, string url)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var emailBody = GetActionEmailBody(user.UserName, url.ToString());
            await  _emailSerivice.SendEmailAsync(user.Email, "Your Content Approval", emailBody.ToString());
            return true;
        }




        private string GetActionEmailBody(string userName, string URL)
        {
            var emailTemplate = System.IO.File.ReadAllText("EmailTemplates/Action.html");
            emailTemplate = emailTemplate.Replace("{UserName}", userName);
            emailTemplate = emailTemplate.Replace("{URL}", URL);
            return emailTemplate;
        }


    }
}
