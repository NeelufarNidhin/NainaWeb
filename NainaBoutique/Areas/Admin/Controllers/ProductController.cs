using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Models.ViewModels;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviroment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnviroment;

        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            List<ProductModel> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(productList);
        }
        public IActionResult Upsert(int? id)
        {
            ProductViewModel productViewModel = new()
            {
                CategoryLlist = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.CategoryName,
                    Value = u.Id.ToString()
                }),
                Product = new ProductModel()
            };
            if(id==0 || id == null)
            {
                //create
                return View(productViewModel);
            }
            else
            {
                //update
                productViewModel.Product = _unitOfWork.Product.Get(u => u.Id==id);
                return View(productViewModel);
            }
           
           // ViewBag.CategoryListAll = CategoryList;
           
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images/product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"/images/product/" + fileName;
                }
                if(productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {

                productVM.CategoryLlist = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.CategoryName,
                    Value = u.Id.ToString()
                });
                   
               
                return View(productVM);
            }
           

        }


        

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ProductModel> objproductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objproductList });
        }

       
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                productToBeDeleted.ImageUrl!.TrimStart('/'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}

