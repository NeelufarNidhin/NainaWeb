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
using NainaBoutique.DataAccess.Data;
using static TheArtOfDev.HtmlRenderer.Adapters.RGraphicsPath;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Customer.Controllers
{
    [Area("Customer")]
    //[Authorize]
    public class CartController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly ApplicationDbContext _db;
        
        [BindProperty]
        public ShoppingCartVM? ShoppingCartVM { get; set; }

        


        public CartController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;




            ShoppingCartVM ShoppingCartVM = new() {

                shoppingCartList = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"
                ),
                OrderSummary = new()

                
                };
            

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

           
                foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                cart.Product!.ProductImage = productImages.Where(u =>u.ProductId==cart.ProductId).ToList();
                cart.Price = cart.Product!.Price;
                ShoppingCartVM.OrderSummary. OrderTotal += (cart.Price * cart.Count);

                
            }
            return View(ShoppingCartVM);
        }


      

        
        




        //increment the counter to increase the count of product
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.Cart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.Cart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        //decrement the counter to decrease the count of product
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


         //   var appliedcoupon = TempData["AppliedCoupon"];

            //string appliedcoupon = HttpContext.Session.GetString("TextboxData");


            ShoppingCartVM = new()
            {

                shoppingCartList = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderSummary = new()

            };


            //  IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();
            ShoppingCartVM.OrderSummary.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


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
                ShoppingCartVM.OrderSummary.OrderTotal += (cart.Price * cart.Count);

            }

       
           return View(ShoppingCartVM);


        }

        [HttpPost]
        public IActionResult ApplyCoupon(string coupon)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            TempData["AppliedCoupon"] = coupon;


            ShoppingCartVM!.shoppingCartList = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId,
                 includeProperties: "Product");

            ShoppingCartVM.OrderSummary.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderSummary.ApplicationUserId = userId;

            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                // cart.Product.ProductImage = productImages.Where(u => u.ProductId == cart.ProductId).ToList();
                cart.Price = cart.Product!.Price;


                ShoppingCartVM.OrderSummary.OrderTotal += (cart.Price * cart.Count);

            }



            //  IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();


            if (coupon != null)
            {
                //checking coupon is there in Applied Coupon Table
                var couponFromDb = _db.AppliedCoupons.FirstOrDefault(u => u.ApplicationUser.Id == userId && u.Coupon.CouponCode == coupon);


                //checking status of the coupon
                CouponModel couponModel = _unitOfWork.Coupon.Get(u => u.CouponCode == coupon);





                if (couponFromDb != null)
                {
                    TempData["error"] = "Coupon alreay used";
                    return RedirectToAction(nameof(Summary));
;                }

                else if (couponFromDb == null)
                {
                    AppliedCoupon appliedCoupon = new AppliedCoupon()
                    {
                        UserId = userId,
                        AppliedStatus = true,
                        CouponId = couponModel.Id

                    };
                    //Discount calculation

                    if (ShoppingCartVM.OrderSummary.OrderTotal >= 250)
                    {
                        var discount = couponModel.Discount;
                        //chk discount less than max discount
                        var maxdiscount = (ShoppingCartVM.OrderSummary.OrderTotal) * (discount / 100);
                        if (maxdiscount <= couponModel.MaxAmount)
                        {
                            ShoppingCartVM.OrderSummary.OrderTotal -= ((ShoppingCartVM.OrderSummary.OrderTotal) * (discount / 100));
                        _db.AppliedCoupons.Add(appliedCoupon);

                        _db.SaveChanges();

                    }
                        else
                        {
                            ShoppingCartVM.OrderSummary.OrderTotal -= 1000;
                        }

                    }

                }
            }
            return View("Summary",ShoppingCartVM);

        }



        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string Paymentmethod )
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var coupon = TempData["AppliedCoupon"];


            ShoppingCartVM!.shoppingCartList = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId,
                 includeProperties: "Product");

            ShoppingCartVM.OrderSummary.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderSummary.ApplicationUserId = userId;

            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                // cart.Product.ProductImage = productImages.Where(u => u.ProductId == cart.ProductId).ToList();
                cart.Price = cart.Product!.Price;


                ShoppingCartVM.OrderSummary.OrderTotal += (cart.Price * cart.Count);

            }
               
            if(Paymentmethod == null ){
                TempData["error"] = "Please select Payment Method";
                // return RedirectToAction(nameof(Summary));
                return View("Summary", ShoppingCartVM);
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
            else if (Paymentmethod == "Wallet")
            {
                ShoppingCartVM.OrderSummary.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderSummary.OrderStatus = SD.StatusPending;
                ShoppingCartVM.OrderSummary.PaymentMethod = "Wallet";
            }






            if (coupon != null)
            {
                //checking coupon is there in Applied Coupon Table
                var couponFromDb = _db.AppliedCoupons.FirstOrDefault(u => u.ApplicationUser.Id == userId && u.Coupon.CouponCode == coupon);


                //checking status of the coupon
                CouponModel couponModel = _unitOfWork.Coupon.Get(u => u.CouponCode == coupon);


                if (couponFromDb != null)
                {

                    //Discount calculation

                    if (ShoppingCartVM.OrderSummary.OrderTotal >= 250)
                    {
                        var discount = couponModel.Discount;
                        //chk discount less than max discount
                        var maxdiscount = (ShoppingCartVM.OrderSummary.OrderTotal) * (discount / 100);
                        if (maxdiscount <= couponModel.MaxAmount)
                        {
                            ShoppingCartVM.OrderSummary.OrderTotal -= ((ShoppingCartVM.OrderSummary.OrderTotal) * (discount / 100));

                        }
                        else
                        {
                            ShoppingCartVM.OrderSummary.OrderTotal -= 1000;
                        }

                    }
                }

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

            else if (Paymentmethod == "Wallet")
            {
                IEnumerable<WalletModel> walletList = _unitOfWork.Wallet.GetAll(u => u.UserId == ShoppingCartVM.OrderSummary.ApplicationUserId);


                var applicationUserDb = _unitOfWork.ApplicationUser.Get(u => u.Id == ShoppingCartVM.OrderSummary.ApplicationUserId);

                var walletbalance = applicationUserDb.WalletBalance;

                var orderSumaryFromDb = _unitOfWork.OrderSummary.Get(u => u.Id == ShoppingCartVM.OrderSummary.Id);
                var amount = ShoppingCartVM.OrderSummary.OrderTotal;


                var amountToPay = walletbalance - amount;

                if (amountToPay >= 0 && amountToPay >= amount)
                {
                    applicationUserDb.WalletBalance = amountToPay;



                    WalletModel wallet = new()
                    {
                        UserId = ShoppingCartVM.OrderSummary.ApplicationUserId,
                        WalletBalance = -amount,
                        OrderId = ShoppingCartVM.OrderSummary.Id
                    };
                    _unitOfWork.ApplicationUser.Update(applicationUserDb);
                    _unitOfWork.Wallet.Update(wallet);



                    _unitOfWork.OrderSummary.UpdateStatus(ShoppingCartVM.OrderSummary.Id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                else
                {
                    TempData["error"] = "Not enough Credit in Wallet , Please choose another Payment Method";
                    return View("Summary", ShoppingCartVM);
                }


            }
            //else if(amountToPay < 0 && amountToPay < amount)
            //{
            //    applicationUserDb.WalletBalance = 0 ;



            //    WalletModel wallet = new()
            //    {
            //        UserId = ShoppingCartVM.OrderSummary.ApplicationUserId,
            //        WalletBalance = 0 ,
            //        OrderId = ShoppingCartVM.OrderSummary.Id
            //    };

            //    var balanceToPay = Math.Abs(amountToPay);

            //    _unitOfWork.ApplicationUser.Update(applicationUserDb);
            //    _unitOfWork.Wallet.Update(wallet);
            //    ShoppingCartVM.OrderSummary.PaymentMethod = "COD";

            //    _unitOfWork.OrderSummary.Update(orderSumaryFromDb);
            //    _unitOfWork.OrderSummary.UpdateStatus(ShoppingCartVM.OrderSummary.Id, SD.StatusApproved, SD.PaymentStatusDelayedPayment);
            //    _unitOfWork.Save();
            //}
            //}

            //_unitOfWork.Save();


            else
            {
                var domain = "https://localhost:7275/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderSummary.Id}",
                    CancelUrl = domain + "Customer/Cart/Index",
                    LineItems = new List<SessionLineItemOptions>(),

                    Mode = "payment",
                    AllowPromotionCodes = true,
                };


                foreach (var item in ShoppingCartVM.shoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {



                        PriceData = new SessionLineItemPriceDataOptions
                        {


                            UnitAmount = (long)(item.Price * 100),
                            Currency = "aed",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {


                                Name = item.Product!.ProductName
                            }
                        },
                        Quantity = item.Count,

                    };

                    options.LineItems.Add(sessionLineItem);

                }
                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderSummary.UpdateStripePayment(ShoppingCartVM.OrderSummary.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
               

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderSummary.Id });
        }

        
       
        
                



        [HttpPost]
        public IActionResult RemoveCoupon()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var coupon = TempData["AppliedCoupon"];
            if (coupon!= null)
            {
                //checking coupon is there in Applied Coupon Table
                var couponFromDb = _db.AppliedCoupons.FirstOrDefault(u => u.ApplicationUser.Id == userId && u.Coupon.CouponCode == coupon);

                _db.AppliedCoupons.Remove(couponFromDb);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Summary));
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

        [HttpPost]
        public IActionResult AddAddress(AddressModel addressModel )
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            AddressModel AddressModel = new()
            {
                UserId = userId,
                Address = addressModel.Address,
                City = addressModel.City,
                State = addressModel.State,
                PostalCode = addressModel.PostalCode,
                MobileNumber = addressModel.MobileNumber


            };

            _db.Address.Add(AddressModel);
            _db.SaveChanges();
            TempData["success"] = "Address Added Successfully";
            // return RedirectToAction("Summary");
            return RedirectToAction(nameof(Index));
          //  return View();
        }


        public IActionResult AddAddress()
        {
            return View();
        }

    }
}

