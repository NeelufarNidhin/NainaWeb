﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.Models.Models;

namespace NainaBoutique.Areas.Identity.Pages.Account
{
    public class VerifyOTPModel : PageModel
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly OtpService _otpService;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext db;

        public VerifyOTPModel(SignInManager<IdentityUser> signInManager, OtpService otpService, ILogger<LoginModel> logger, ApplicationDbContext _db)
        {
            _otpService = otpService;
            _signInManager = signInManager;
            _logger = logger;
            db = _db;


        }

        //[BindProperty]
        //public OtpModel otpModel { get; set; }


      
           [BindProperty]
            public string Email { get; set; }

           [BindProperty]
            public string? OTP { get; set; }


       



        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email,string OTP)


        {

            //db.OtpModels.Add(u=>u)

           // var otpfromBb = db.OtpModels.FirstOrDefault(u => u.Email == otpModel.Email);
            if (_otpService.ValidateOtp(email,OTP))
            {

                var user = new IdentityUser { UserName = email };
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
