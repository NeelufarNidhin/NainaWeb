using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class ProfileController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ProfileVM ProfileVM { get; set; }

        public ProfileController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
        // GET: /<controller>/
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ProfileVM = new()
            {

                ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId)
            };

            return View(ProfileVM);
        }

        [HttpPost]

        public IActionResult UpdateProfile(int userId)
        {

            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == ProfileVM.ApplicationUser.Id);
            user.UserName = ProfileVM.ApplicationUser.UserName;
            user.MobileNumber = ProfileVM.ApplicationUser.MobileNumber;
            user.Address = ProfileVM.ApplicationUser.Address;
            user.City = ProfileVM.ApplicationUser.City;
            user.State = ProfileVM.ApplicationUser.State;
            user.PostalCode = ProfileVM.ApplicationUser.PostalCode;

            _unitOfWork.ApplicationUser.Update(user);

            TempData["success"] = "Profile Updated Successfully";
            _unitOfWork.Save();

            return Redirect("~/");


        }
    }
}

