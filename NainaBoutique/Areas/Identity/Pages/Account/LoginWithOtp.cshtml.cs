using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static NainaBoutique.Areas.Identity.Pages.Account.VerifyOTPModel;

namespace NainaBoutique.Areas.Identity.Pages.Account
{
	public class LoginWithOtpModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        private readonly OtpService _otpService;

        public LoginWithOtpModel(IEmailSender emailSender, OtpService otpService)
        {
            _emailSender = emailSender;
            _otpService = otpService;



        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string? OTP { get; set; }




        public IActionResult
            OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)

            {
                string otp = _otpService.GenerateRandomOtp();
                _otpService.StoreOtp(Email, otp);



                await SendEmailAsync(Email, "OTP", otp);

                return RedirectToPage("./VerifyOtp");

            }

            return Page();
        }


        public Task<bool> SendEmailAsync(string email, string subject, string otp)
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
                message.Body = otp;

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

    public class OtpService
    {
        private Dictionary<string, string> otpStorage = new Dictionary<string, string>();

        public string GenerateRandomOtp()
        {
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            return otp;
        }

        public void StoreOtp(string email, string otp)
        {
            otpStorage[email] = otp;
        }

        public bool ValidateOtp(string email, string otp)
        {
            if (otpStorage.TryGetValue(email, out string storedOtp) && otp == storedOtp)
            {
                otpStorage.Remove(email);
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
