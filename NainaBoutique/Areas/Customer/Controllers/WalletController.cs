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
using Stripe.Checkout;

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
                IEnumerable<WalletTopUp> walletTopUpList = _unitOfWork.WalletTopUp.GetAll(u => u.Userid == userId);


            var WalletTotal = walletList.Sum(u => u.WalletBalance) + walletTopUpList.Sum(u => u.WalletBalance);

            applicationUserDb.WalletBalance = WalletTotal;

                _unitOfWork.ApplicationUser.Update(applicationUserDb);
                _unitOfWork.Save();

                ViewBag.Message = WalletTotal.ToString();

            //}
            return View();
        }

        public IActionResult TopUpWallet(string amount)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            
            if (amount != null)
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>(),

                    Mode = "payment",
                    SuccessUrl = domain + "Customer/Wallet/Success",
                    CancelUrl = domain + "Customer/Wallet/Index",
                };

                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(float.Parse(amount) * 100),
                        Currency = "aed",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Recharge Wallet"
                        }
                    },
                    Quantity = 1
                };

                options.LineItems.Add(sessionLineItem);

                var service = new SessionService();
                ///  service.Create(options);
                Session session = service.Create(options);


                WalletTopUp wallettopup = new()
                {
                    Userid = userId,
                    WalletBalance = float.Parse(amount),
                    PurchaseDate = DateTime.Now
                };


               

                Response.Headers.Add("Location", session.Url);
                _unitOfWork.WalletTopUp.Add(wallettopup);

                _unitOfWork.Save();
                return new StatusCodeResult(303);
               
                //return View();
            }
           
            return RedirectToAction("Success");

        }

        public IActionResult Success()
        {
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


