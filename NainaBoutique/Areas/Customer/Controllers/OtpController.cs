using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Customer.Controllers
{
    public class OtpController : Controller
    {

        // Generate a random OTP (e.g., a 6-digit number)
        public static Random random = new Random();
        string otp = random.Next(100000, 999999).ToString();
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public async Task <IActionResult> SendMail(string email)
        { 
            // var userId = await _userManager.GetUserIdAsync(user);

            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
               // Generate a random OTP (e.g., a 6-digit number)
       

            await SendEmailAsync(email, "Confirm your email",
                   //   otp);
                   $"Please confirm your account by <a href={otp}>clicking here</a>.");

            return View();
        }


        private Task<bool> SendEmailAsync(string email, string subject, string messageotp)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                message.From = new MailAddress("albyjolly149@gmail.com");
                message.To.Add(email);
                message.Subject = subject;
                message.IsBodyHtml = true;
               // message.Body = confirmurl;
                 message.Body = messageotp;

                smtpClient.Port = 587;
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("albyjolly149@gmail.com", "ieivzgnukcrjdape");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(message);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {

                return Task.FromResult(false);
            }
        }
    }
}

