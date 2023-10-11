using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Models.Models;
using NainaBoutique.Models.ViewModels;

namespace NainaBoutique.Areas.Customer.Controllers;
[Area("Customer")]

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;


    private readonly IEmailSender emailSender;

    public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork, IEmailSender sender)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        emailSender = sender;

    }


    public class Emailsender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("neelufar.nidhin@gmail.com", "ixobimgtkbsijgyk", "smtp.gmail.com")
            };

            return client.SendMailAsync(
                new MailMessage(from: "neelufar.nidhin@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }




    //public IActionResult Index()
        
    //{
    //    IEnumerable<ProductModel> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImage");



    //    return View(productList);
    //}

    public async Task<IActionResult> Index(string searchString, string color,List< string> size, List<string> price, string sortOrder)
    {

        ViewData["CurrentFilter"] = searchString;

    
        IEnumerable<ProductModel> productList = _unitOfWork.Product.GetAll(u=>u.QuantityInStock > 0 && u.RecStatus=='A',includeProperties: "Category,ProductImage");



        if (sortOrder != null)
        {
            
           


            switch (sortOrder)
            {
                case "priceLowToHigh":
                    productList = productList.OrderBy(u => u.Price);
                   
                    break;
                case "priceHighToLow":
                    productList = productList.OrderByDescending(u => u.Price);
                   
                    break;
                case "nameAZ":
                    productList = productList.OrderBy(u => u.ProductName);
                   
                    break;
                case "nameZA":
                    productList = productList.OrderByDescending(u => u.ProductName);
                   
                    break;
                case "featured":
                    productList = productList.OrderBy(u => u.CreatedAt);

                    break;
                default:
                    productList = productList.OrderBy(u => u.CreatedAt);

                    break;
            }
            return View(productList);
        }


        var priceRanges = new List<string>();

        foreach (var p in price)
        {
            if (p == "0-50")
            {
                var sprice = p.Split('-');
                priceRanges.Add(sprice[0]+ ".00");
                priceRanges.Add(sprice[1]+ ".00");

            }
            if (p == "50-100")
            {
                var sprice = p.Split('-');
                priceRanges.Add(sprice[0] + ".00");
                priceRanges.Add(sprice[1]+".00"); ;
            }
            if (p == "100-250")
            {
                var sprice = p.Split('-');
                priceRanges.Add(sprice[0] + ".00");
                priceRanges.Add(sprice[1]+".00"); ;
            }
            if (p == "250-500")
            {
                var sprice = p.Split('-');
                priceRanges.Add(sprice[0] + ".00");
                priceRanges.Add(sprice[1]+".00"); ;
            }
            if (p == "500-1000")
            {
                var sprice = p.Split('-');
                priceRanges.Add(sprice[0] + ".00");
                priceRanges.Add(sprice[1]+".00"); ;
            }
        }




        if (!String.IsNullOrEmpty(searchString))
        {
           productList = productList.Where(s => s.ProductName.ToLower().Contains(searchString.ToLower())
           || s.Category.CategoryName.ToLower().Contains(searchString.ToLower()) || s.Price.ToString().Contains(searchString)||
           s.Sale_Price.ToString()!.Contains(searchString)|| s.Color.ToLower().Contains(searchString.ToLower())
          );
            return View(productList);
        }


        if(color != null)
        {
            productList = productList.Where(s => s.Color.ToLower() == color);
            return View(productList);
        }
        

        if(size!=null && size.Any())
        {
            productList = productList.Where(u => size.Contains(u.Size));
            return View(productList);
        }
        

        if (priceRanges!= null && priceRanges.Any())
        {
            
            productList = productList.Where(s => s.Price >= float.Parse(priceRanges[0]) && s.Price <= float.Parse(priceRanges[1]));
            return View(productList);
        }
        else
        {
            return View(productList);
        }
           }



    [HttpPost]
    public string Index(string searchString, bool notUsed)
    {
        return "From [HttpPost]Index: filter on " + searchString;
    }

    


    public IActionResult Details(int productId)

    {
       
        ShoppingCart shoppingCart = new ShoppingCart()
        {
            Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,ProductImage"),
            Count = 1,
            ProductId = productId,
      

        };
       
        return View(shoppingCart);
    }


   
   

    [HttpPost]
    [Authorize]
    public IActionResult AddtoCart(ShoppingCart shoppingCart)

    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;

        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        shoppingCart.ApplicationUserId = userId;

        ShoppingCart cartFomDb = _unitOfWork.Cart.Get(u => u.ApplicationUserId ==shoppingCart.ApplicationUserId &&  u.ProductId== shoppingCart.ProductId);

        var favouriteFromDb = _unitOfWork.Favourite.Get(u => u.ProductId == shoppingCart.ProductId);
        if (cartFomDb != null){
            //cart exists
          cartFomDb.Count += shoppingCart.Count;
            _unitOfWork.Cart.Update(cartFomDb);
          

        }
        else
        {
           // favouriteFromDb.Count = 1;
            _unitOfWork.Cart.Add(shoppingCart);
            if(favouriteFromDb != null)
            {
                _unitOfWork.Favourite.Remove(favouriteFromDb);
            }
           

        }
        TempData["success"] = "Cart Updated Successfully";
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [Authorize]
    public IActionResult AddtoFav(FavouritesModel favouritesModel)

    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;

        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        favouritesModel.ApplicationUserId = userId;

        FavouritesModel favouritesFomDb = _unitOfWork.Favourite.Get(u => u.ApplicationUserId == favouritesModel.ApplicationUserId && u.ProductId == favouritesModel.ProductId);
     

        _unitOfWork.Favourite.Add(favouritesModel);
        
       

        TempData["success"] = "Favourites Updated Successfully";
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }


    public IActionResult Privacy()
    {
        return View();
    }

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
   


}




