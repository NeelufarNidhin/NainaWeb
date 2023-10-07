using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Models.Models;
using NainaBoutique.Models.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class WalletController : Controller
    {

        public readonly IUnitOfWork _unitOfWork;



        //[BindProperty]
        //public WalletVM? WalletVM { get; set; }

        public WalletController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            //if (ModelState != null)
            //{



                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


                var applicationUserDb = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
                IEnumerable<WalletModel> walletList = _unitOfWork.Wallet.GetAll(u => u.UserId == userId);


                var WalletTotal = walletList.Sum(u => u.WalletBalance);

                applicationUserDb.WalletBalance = WalletTotal;

                _unitOfWork.ApplicationUser.Update(applicationUserDb);
                _unitOfWork.Save();

                ViewBag.Message = WalletTotal.ToString();

            //}
            return View();
        }



        


        #region API CALLS


        [HttpGet]
        public IActionResult GetAll()
        {

            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

          var objwalletModels = _unitOfWork.Wallet.GetAll(u => u.UserId == userId);

            return Json(new { data = objwalletModels });
        }

        #endregion

    }
}


