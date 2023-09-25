using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Repository;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.ViewModels;
using NainaBoutique.Utility;
using NainaBoutique.Models.Models;
using Stripe.Checkout;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;
using Stripe;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Customer.Controllers
{
    [Area("Customer")]
    //[Authorize]
    public class CartController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

       
        //[Required(ErrorMessage = "Please select the Payment Method")]
        //public string? PaymentMethod { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;  
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            

            ShoppingCartVM = new() {

                shoppingCartList = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"
                ),
                OrderSummary = new()

                
                };
            

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

           

          

                foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                cart.Product.ProductImage = productImages.Where(u =>u.ProductId==cart.ProductId).ToList();
                cart.Price = cart.Product!.Price;
                ShoppingCartVM.OrderSummary. OrderTotal += (cart.Price * cart.Count);

                
            }
            return View(ShoppingCartVM);
        }


        [HttpPost]
        public IActionResult Index( string coupon)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;




            ShoppingCartVM = new()
            {

                shoppingCartList = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"
                ),
                OrderSummary = new()


            };


            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

           



            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                cart.Product.ProductImage = productImages.Where(u => u.ProductId == cart.ProductId).ToList();
                cart.Price = cart.Product!.Price;

                if (coupon != null)
                {
                    CouponModel couponModel = _unitOfWork.Coupon.Get(u => u.CouponCode == coupon);
                    var discount = couponModel.Discount;
                    ShoppingCartVM.OrderSummary.OrderTotal = (ShoppingCartVM.OrderSummary.OrderTotal + (cart.Price * cart.Count)) -
                        ((ShoppingCartVM.OrderSummary.OrderTotal + (cart.Price * cart.Count)) * discount / 100);
                        
                }
                else
                {
                    ShoppingCartVM.OrderSummary.OrderTotal = (ShoppingCartVM.OrderSummary.OrderTotal + (cart.Price * cart.Count));
                }



            }
            return View(ShoppingCartVM);
        }



        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.Cart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.Cart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.Cart.Get(u => u.Id == cartId);
            if(cartFromDb.Count <= 1){
                _unitOfWork.Cart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.Cart.Update(cartFromDb);
            }
           
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.Cart.Get(u => u.Id == cartId);
            _unitOfWork.Cart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary(string coupon)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;




            ShoppingCartVM = new()
            {

                shoppingCartList = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderSummary = new()

            };


            //  IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();
            ShoppingCartVM.OrderSummary.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id ==userId);


            ShoppingCartVM.OrderSummary.Name = ShoppingCartVM.OrderSummary.ApplicationUser.Name;
            ShoppingCartVM.OrderSummary.MobileNumber = ShoppingCartVM.OrderSummary.ApplicationUser.MobileNumber;
            ShoppingCartVM.OrderSummary.Address = ShoppingCartVM.OrderSummary.ApplicationUser.Address;
            ShoppingCartVM.OrderSummary.City = ShoppingCartVM.OrderSummary.ApplicationUser.City;
            ShoppingCartVM.OrderSummary.State = ShoppingCartVM.OrderSummary.ApplicationUser.State;
            ShoppingCartVM.OrderSummary.PostalCode = ShoppingCartVM.OrderSummary.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                // cart.Product.ProductImage = productImages.Where(u => u.ProductId == cart.ProductId).ToList();
                cart.Price = cart.Product!.Price;


                if (coupon != null)
                {
                    CouponModel couponModel = _unitOfWork.Coupon.Get(u => u.CouponCode == coupon);
                    var discount = couponModel.Discount;
                    ShoppingCartVM.OrderSummary.OrderTotal = (ShoppingCartVM.OrderSummary.OrderTotal + (cart.Price * cart.Count)) -
                        ((ShoppingCartVM.OrderSummary.OrderTotal + (cart.Price * cart.Count)) * discount / 100);

                }
                else
                {
                    ShoppingCartVM.OrderSummary.OrderTotal = (ShoppingCartVM.OrderSummary.OrderTotal + (cart.Price * cart.Count));
                }

                // ShoppingCartVM.OrderSummary.OrderTotal += (cart.Price * cart.Count);

            }


            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string Paymentmethod)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;




            ShoppingCartVM.shoppingCartList = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId,
                 includeProperties: "Product");

            ShoppingCartVM.OrderSummary.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderSummary.ApplicationUserId = userId;
            //ShoppingCartVM.OrderSummary.PaymentIntendId = "COD";
           
            //  IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();
          //  ShoppingCartVM.OrderSummary.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


            
            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                // cart.Product.ProductImage = productImages.Where(u => u.ProductId == cart.ProductId).ToList();
                cart.Price = cart.Product!.Price;


                 ShoppingCartVM.OrderSummary.OrderTotal += (cart.Price * cart.Count);
            }


           
                if (Paymentmethod == "COD")
                {
                    ShoppingCartVM.OrderSummary.PaymentStatus = SD.PaymentStatusDelayedPayment;
                    ShoppingCartVM.OrderSummary.OrderStatus = SD.StatusApproved;
                    ShoppingCartVM.OrderSummary.PaymentMethod = "COD";
                }
                else if (Paymentmethod == "Card")
                {
                    ShoppingCartVM.OrderSummary.PaymentStatus = SD.PaymentStatusPending;
                    ShoppingCartVM.OrderSummary.OrderStatus = SD.StatusPending;
                    ShoppingCartVM.OrderSummary.PaymentMethod = "Card";
                }
            
            
            
            _unitOfWork.OrderSummary.Add(ShoppingCartVM.OrderSummary);
            _unitOfWork.Save();
            
            //Order Detail

            foreach(var cart in ShoppingCartVM.shoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderSummaryId = ShoppingCartVM.OrderSummary.Id,
                    Price = cart.Price,

                    Count = cart.Count

                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            //  return View(ShoppingCartVM);

            if (Paymentmethod == "COD")
            {

                return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderSummary.Id });
            }
           // return RedirectToAction(nameof(OrderCheckout));


            else  
            {
                var domain = "https://localhost:7275/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderSummary.Id}",
                    CancelUrl = domain + "Customer/Cart/Index",
                    LineItems = new List<SessionLineItemOptions>(),
                  
                    Mode = "payment",
                };


                foreach (var item in ShoppingCartVM.shoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions { 
                    


                        PriceData = new SessionLineItemPriceDataOptions { 
                        

                            UnitAmount = (long)(item.Price * 100) ,
                            Currency = "aed",
                            ProductData = new SessionLineItemPriceDataProductDataOptions { 
                            

                                Name = item.Product.ProductName
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new SessionService();
               Session session= service.Create(options);
                _unitOfWork.OrderSummary.UpdateStripePayment(ShoppingCartVM.OrderSummary.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);


                return new StatusCodeResult(303);
            }
            // return View(ShoppingCartVM);
        }

        public IActionResult OrderConfirmation(OrderSummary order)
        {
           var orderSummary = _unitOfWork.OrderSummary.Get(u => u.Id == order.Id, includeProperties: "ApplicationUser");

            if(orderSummary.PaymentMethod == "Card")
            {
                var service = new SessionService();
                Session session = service.Get(orderSummary.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderSummary.UpdateStripePayment(order.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderSummary.UpdateStatus(order.Id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
               // HttpContext.Session.Clear();
            }



            List<ShoppingCart> shoppingCarts = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == orderSummary.ApplicationUserId).ToList();
            _unitOfWork.Cart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(order);

        }

        

    }
}

