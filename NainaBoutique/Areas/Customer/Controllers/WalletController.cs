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
    public class WalletController : Controller
    {

        public readonly IUnitOfWork _unitOfWork;



        [BindProperty]
        public WalletVM? WalletVM { get; set; }

        public WalletController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
           
        }
        // GET: /<controller>/
        public IActionResult Index()
        {


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            WalletVM = new WalletVM() { 
            
                WalletList = _unitOfWork.Wallet.GetAll(u => u.ApplicationUser.Id == userId)
           };


            //foreach(var wallet in WalletVM.WalletList)
            //{
            //    wallet.WalletBalance += wallet.WalletBalance;
            //}

           



            return View(WalletVM);

        }
    }
}

