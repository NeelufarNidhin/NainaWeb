using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{
    [Area("Customer")]
    public class FavouritesController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public FavouritesVM FavouritesVM { get; set; }
        public FavouritesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;




            FavouritesVM = new()
            {

                favouritesList = _unitOfWork.Favourite.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product")
            };

            
            return View(FavouritesVM);
        }
        public IActionResult Remove(int favouritesId)
        {
            var favouriteFromDb = _unitOfWork.Favourite.Get(u => u.Id == favouritesId);
            _unitOfWork.Favourite.Remove(favouriteFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // GET: /<controller>/
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}

