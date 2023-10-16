using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.ViewModels;
using Stripe;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Customer.Controllers
{

    [Area("Customer")]
    [Authorize]
    public class AddressController : Controller
    {

        public readonly IUnitOfWork _unitOfWork;
        public AddressController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            AddressVM  AddressVM= new()
            {
                Address = _unitOfWork.Address.GetAll(u => u.UserId == userId  && u.Status == 0)
            };
            return View(AddressVM);
        }


        [HttpPost]
        public IActionResult SelectAddress(int addressId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var addressFromDb = _unitOfWork.Address.Get(u => u.Id == addressId && u.UserId== userId);
            if (addressFromDb != null)
            {
                addressFromDb.Status = 1;
                _unitOfWork.Address.Update(addressFromDb);
            }

            var addressFromDb1 = _unitOfWork.Address.Get(u => u.Id != addressId && u.UserId == userId && u.Status == 1);
            if (addressFromDb1 != null)
            {
                addressFromDb1.Status = 0;
                _unitOfWork.Address.Update(addressFromDb1);
            }

            _unitOfWork.Save();

            return RedirectToAction("Summary", "Cart");
        }





    }
}
    


