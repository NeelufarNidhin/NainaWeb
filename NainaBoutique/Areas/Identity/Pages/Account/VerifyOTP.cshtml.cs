﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NainaBoutique.Areas.Identity.Pages.Account
{
    public class VerifyOTPModel : PageModel
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly OtpService _otpService;
        private readonly ILogger<LoginModel> _logger;

        public VerifyOTPModel(SignInManager<IdentityUser> signInManager, OtpService otpService, ILogger<LoginModel> logger)
        {
            _otpService = otpService;
            _signInManager = signInManager;
            _logger = logger;


        }
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string OTP { get; set; }


        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()


        {
            if (_otpService.ValidateOtp(Email, OTP))
            {

                var user = new IdentityUser { UserName = Email, Email = Email };
                var result = await _signInManager.PasswordSignInAsync(user, OTP, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect("~/");

                    // return RedirectToPage("~/");
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid OTP or Login Failed");
                }
            }

            else
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP");
            }
            return Page();
        }
        
    }
}