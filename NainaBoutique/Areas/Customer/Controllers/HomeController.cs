using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
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
                Credentials = new NetworkCredential("albyjolly149@gmail.com", "ieivzgnukcrjdape", "smtp.gmail.com")
            };

            return client.SendMailAsync(
                new MailMessage(from: "albyjolly149@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }




    public IActionResult Index()
        
    {
        IEnumerable<ProductModel> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
        return View(productList);
    }

   
    public IActionResult Details(int productId)

    {
       
        ShoppingCart shoppingCart = new ShoppingCart()
        {
            Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
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

        if(cartFomDb != null){
            //cart exists
          cartFomDb.Count += shoppingCart.Count;
            _unitOfWork.Cart.Update(cartFomDb);
          

        }
        else
        {
            _unitOfWork.Cart.Add(shoppingCart);
            
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

    //public async Task<IActionResult> Privacy()
    //{
    //    var email = "nehanmohammed@gmail.com";
    //    var subject = "otp";
    //    var message = "34567";
    //    await emailSender.SendEmailAsync(email, subject, message);
    //    return View();
    //}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
   


}




