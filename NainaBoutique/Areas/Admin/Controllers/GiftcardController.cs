using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;
using NainaBoutique.Models.ViewModels;
using NainaBoutique.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class GiftcardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
       // public GiftcardVM GiftcardVM { get; set; }

        public GiftcardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            List<GiftcardModel> giftcardModels = _unitOfWork.Giftcard.GetAll().ToList();
            return View(giftcardModels);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(GiftcardModel giftcard)
        {
            if (ModelState.IsValid)
            {

               // checking whether category exists
                GiftcardModel cardName = _unitOfWork.Giftcard.Get(u => u.CardNumber == giftcard.CardNumber);


                if (cardName != null)
                {

                    TempData["error"] = "Gitfcard Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Giftcard.Add(giftcard);
                    _unitOfWork.Save();
                    TempData["success"] = "Giftcard Created Successfully";
                    return RedirectToAction("Index");
                }

            }
            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            GiftcardModel? giftcardFromDb = _unitOfWork.Giftcard.Get(u => u.Id == id);
            if (giftcardFromDb == null)
            {
                return NotFound();
            }
            return View(giftcardFromDb);
        }
        [HttpPost]
        public IActionResult Edit(GiftcardModel giftcard)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Giftcard.Update(giftcard);
                _unitOfWork.Save();
                TempData["success"] = "Giftcard Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<GiftcardModel> giftcardList = _unitOfWork.Giftcard.GetAll().ToList();
            return Json(new { data = giftcardList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var giftcardToBeDeleted = _unitOfWork.Giftcard.Get(u => u.Id == id);
            if (giftcardToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Giftcard.Remove(giftcardToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successfull" });
        }
        #endregion
    }
}

