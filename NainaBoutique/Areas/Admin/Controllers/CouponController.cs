using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Repository;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Models.Models;
using NainaBoutique.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CouponController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        // GET: /<controller>/

        public CouponController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<CouponModel> couponModel = _unitOfWork.Coupon.GetAll().ToList();
            return View(couponModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CouponModel coupon)
        {
            if (ModelState.IsValid)
            {

                // checking whether giftcard exists
                CouponModel couponCode = _unitOfWork.Coupon.Get(u => u.CouponCode == coupon.CouponCode);


                if (couponCode != null)
                {

                    TempData["error"] = "Coupon Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Coupon.Add(coupon);
                    _unitOfWork.Save();
                    TempData["success"] = "Coupon Created Successfully";
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
            CouponModel couponFromDb = _unitOfWork.Coupon.Get(u => u.Id == id);
            if (couponFromDb == null)
            {
                return NotFound();
            }
            return View(couponFromDb);
        }
        [HttpPost]
        public IActionResult Edit(CouponModel coupon)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Coupon.Update(coupon);
                _unitOfWork.Save();
                TempData["success"] = "Coupon Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        #region API CALLS


        [HttpGet]
        public IActionResult GetAll()
        {
            List<CouponModel> objcouponList = _unitOfWork.Coupon.GetAll().ToList();
            return Json(new { data = objcouponList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var couponToBeDeleted = _unitOfWork.Coupon.Get(u => u.Id == id);
            if (couponToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Coupon.Remove(couponToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successfull" });
        }


        #endregion
    }

}