using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            List<CategoryModel> categoryList = _unitOfWork.Category.GetAll().ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CategoryModel category)
        {
            if(ModelState.IsValid)
                {

                //checking whether category exists
                CategoryModel categoryName =_unitOfWork.Category.Get(u => u.CategoryName == category.CategoryName);


                if (categoryName != null)
                {
                   
                    TempData["error"] = "Category Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Category.Add(category);
                    _unitOfWork.Save();
                    TempData["success"] = "Category Created Successfully";
                    return RedirectToAction("Index");
                }
                
            }
            return View();
            
        }

        public IActionResult Edit(int? id)
        {
            if(id==null|| id == 0)
            {
                return NotFound();
            }
            CategoryModel? categoryFromDb = _unitOfWork.Category.Get(u=>u.Id==id);
            if(categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();

        }
        

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<CategoryModel> objcategoryList = _unitOfWork.Category.GetAll().ToList();
            return Json(new { data = objcategoryList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var categoryToBeDeleted = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Category.Remove(categoryToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successfull" });
        }
        #endregion


    }
}
