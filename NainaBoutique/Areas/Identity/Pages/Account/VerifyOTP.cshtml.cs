using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;
using NainaBoutique.Utility;

namespace NainaBoutique.Areas.Identity.Pages.Account
{
    public class VerifyOTPModel : PageModel
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly OtpService _otpService;
        private readonly ILogger<LoginModel> _logger;
        private readonly IUnitOfWork _unitOfWork;

       

        public VerifyOTPModel(SignInManager<IdentityUser> signInManager, OtpService otpService,
            ILogger<LoginModel> logger, IUnitOfWork unitOfWork)
        {
            _otpService = otpService;
            _signInManager = signInManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
           

        }

        [BindProperty]
        public OtpModels Input { get; set; }


        public class OtpModels
        {
            

            [Required]
            public string? Email { get; set; }

            [Required]
            public string? OTP { get; set; }


            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }



        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()


        {

          
            var user = _unitOfWork.ApplicationUser.Get(u => u.Email == Input.Email);
            if (_otpService.ValidateOtp(Input.Email, Input.OTP))
            {

               // var user = new IdentityUser { UserName = Input. Email = Input. Email };
                 await _signInManager.SignInAsync(user, isPersistent:false);

                
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect("~/");

                
            }

            else
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP or Login Failed");
            }
            return Page();
        }
        
    }
}
