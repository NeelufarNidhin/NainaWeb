using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Models.Models;
using NainaBoutique.Models.ViewModels;
using NainaBoutique.Utility;
using Stripe;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrderController : Controller
    {

        public readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM  = new() { 
           

                OrderSummary = _unitOfWork.OrderSummary.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderSummaryId == orderId, includeProperties: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles=SD.Role_Admin)]
        public IActionResult UpdateOrderDetail(int orderId)
        {
            var orderSummaryFromDb = _unitOfWork.OrderSummary.Get(u => u.Id == OrderVM.OrderSummary.Id);

            orderSummaryFromDb.Name = OrderVM.OrderSummary.Name;
            orderSummaryFromDb.MobileNumber = OrderVM.OrderSummary.MobileNumber;
            orderSummaryFromDb.Address = OrderVM.OrderSummary.Address;
            orderSummaryFromDb.City = OrderVM.OrderSummary.City;
            orderSummaryFromDb.State = OrderVM.OrderSummary.State;
            orderSummaryFromDb.PostalCode = OrderVM.OrderSummary.PostalCode;

            if (!string.IsNullOrEmpty(OrderVM.OrderSummary.Carrier))
            {
                orderSummaryFromDb.Carrier = OrderVM.OrderSummary.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderSummary.TrackingNumber))
            {
                orderSummaryFromDb.TrackingNumber = OrderVM.OrderSummary.TrackingNumber;
            }


            _unitOfWork.OrderSummary.Update(orderSummaryFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new { orderId = orderSummaryFromDb.Id });
        }



        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult StartProcessing()
        {

            _unitOfWork.OrderSummary.UpdateStatus(OrderVM.OrderSummary.Id, SD.StatusInProcess);

            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderSummary.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult ShipOrder()
        {

            var orderSummary = _unitOfWork.OrderSummary.Get(u => u.Id == OrderVM.OrderSummary.Id);
            
            orderSummary.TrackingNumber = OrderVM.OrderSummary.TrackingNumber;
            orderSummary.Carrier = OrderVM.OrderSummary.Carrier;
            orderSummary.OrderStatus = SD.StatusShipped;
            orderSummary.ShippingDate = DateTime.Now;

            if(orderSummary.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderSummary.PaymentDueDate =DateTime.Now.AddDays(14);
            }
            


            _unitOfWork.OrderSummary.UpdateStatus(OrderVM.OrderSummary.Id,SD.StatusShipped);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderSummary.Id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CancelOrder()
        {
            var orderSummary = _unitOfWork.OrderSummary.Get(u => u.Id == OrderVM.OrderSummary.Id);


            var orderDetail = _unitOfWork.OrderDetail.Get(u => u.Id == orderSummary.Id);
           

            if (orderSummary.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions { 
               
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderSummary.PaymentIntendId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderSummary.UpdateStatus(orderSummary.Id, SD.StatusCancelled, SD.StatusRefunded);

            }

            else
            {
                _unitOfWork.OrderSummary.UpdateStatus(orderSummary.Id, SD.StatusCancelled, SD.StatusCancelled);

            }


            _unitOfWork.Save();
            TempData["Success"] = "Order  Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderSummary.Id });


            //if (orderSummary.OrderStatus != "Cancelled" || orderSummary.OrderStatus != "Refunded")
            //{

            //    orderDetail.Product.QuantityInStock = orderDetail.Product.QuantityInStock - orderDetail.Count;
            //    _unitOfWork.Product.Update(orderDetail.Product);


            //}


        }


        [ActionName("Details")]
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult PayNow()
        {
            var orderSummary = _unitOfWork.OrderSummary.Get(u => u.Id == OrderVM.OrderSummary.Id);

            orderSummary.PaymentDate = DateTime.Now;

            _unitOfWork.OrderSummary.UpdateStatus(orderSummary.Id, SD.StatusShipped, SD.PaymentStatusCompleted);
            _unitOfWork.Save();
            TempData["Success"] = "COD Payment Completed Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderSummary.Id });
        }
            #region API CALLS


            [HttpGet]
        public IActionResult GetAll(string status )
        {
            IEnumerable<OrderSummary> objOrderSummaryList = _unitOfWork.OrderSummary.GetAll(includeProperties: "ApplicationUser").ToList();


            if (User.IsInRole(SD.Role_Admin))
            {
                objOrderSummaryList = _unitOfWork.OrderSummary.GetAll(includeProperties: "ApplicationUser").ToList();
            }

            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderSummaryList = _unitOfWork.OrderSummary.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }

            switch (status)
            {

                case "pending":
                    objOrderSummaryList = objOrderSummaryList.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderSummaryList = objOrderSummaryList.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderSummaryList = objOrderSummaryList.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderSummaryList = objOrderSummaryList.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    
                    break;
            }

            return Json(new { data = objOrderSummaryList });
        }

        




        #endregion

    }
}

