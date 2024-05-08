using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Models.Models;
using NainaBoutique.Models.ViewModels;
using NainaBoutique.Utility;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
            if (id == 0 || id == null)
            {
                //create
                return View(productViewModel);
            }
            else
            {
                //update
                productViewModel.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties:"ProductImage");
                return View(productViewModel);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {

                if (productVM.Product!.Id == 0)
                {
                    ProductModel productName = _unitOfWork.Product.Get(u => u.ProductName == productVM.Product.ProductName);
                    ProductModel productSize = _unitOfWork.Product.Get(u => u.Size == productVM.Product.Size);

                    if(productName != null && productSize != null)
                    {
                        TempData["error"] = "Product Already Exists";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _unitOfWork.Product.Add(productVM.Product);
                    }
                   
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

            

            _unitOfWork.Save();
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if (files != null)
            {

                foreach (IFormFile file in files)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = @"images/products/product-" + productVM.Product.Id;
                    string finalPath = Path.Combine(wwwRootPath, productPath);


                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create)) {

                        file.CopyTo(fileStream);
                    }


                    ProductImage productImage = new()
                    {

                        ImageUrl = @"/" + productPath + @"/" + fileName,
                        ProductId = productVM.Product.Id
                    };


                    if (productVM.Product.ProductImage == null)
                        productVM.Product.ProductImage = new List<ProductImage>();

                    productVM.Product.ProductImage.Add(productImage);


                }


                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();


            }
            TempData["success"] = "Product Create/Updated Successfully";


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

        public IActionResult DeleteImage(int? imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                imageToBeDeleted.ImageUrl!.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted successfully";

            }



            return RedirectToAction(nameof(Upsert), new { id = productId });
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ProductModel> objproductList = _unitOfWork.Product.GetAll(u => u.RecStatus =='A', includeProperties: "Category").ToList();
            return Json(new { data = objproductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }


            string productPath = @"images/products/product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);


            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach(string filePath in filePaths)
                {
                   System.IO.File.Delete(filePath);
                 
                }
               Directory.Delete(finalPath);

            }
                
            
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successfull" });
        }




        #endregion
    }
}

