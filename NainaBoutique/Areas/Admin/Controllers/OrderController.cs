using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Models.Models;
using NainaBoutique.Models.ViewModels;
using NainaBoutique.Utility;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using Stripe;
using TheArtOfDev.HtmlRenderer.PdfSharp;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrderController : Controller
    {

        public readonly IUnitOfWork _unitOfWork;
        public readonly ApplicationDbContext _db;


       [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
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
        
        [Authorize(Roles = SD.Role_User)]
        public IActionResult CancelOrder()
        {
            var orderSummary = _unitOfWork.OrderSummary.Get(u => u.Id == OrderVM.OrderSummary.Id);

            //Stock Management 
            var orderDetail = _unitOfWork.OrderDetail.Get(u => u.Id == orderSummary.Id);
            var productFromDb = _unitOfWork.Product.Get(u => u.Id == orderDetail.ProductId);

            productFromDb.QuantityInStock = productFromDb.QuantityInStock + orderDetail.Count;
            _unitOfWork.Product.Update(productFromDb);
           
            //Card Payment .Refund to card
            if (orderSummary.PaymentStatus == SD.PaymentStatusApproved ) 
            {
                var options = new RefundCreateOptions
                {

                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderSummary.PaymentIntendId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);
                _unitOfWork.OrderSummary.UpdateStatus(orderSummary.Id, SD.StatusCancelled, SD.StatusRefunded);
            }


            //COD Refund to wallet
            if (orderSummary.PaymentStatus == SD.PaymentStatusDelayedPayment)
            { 
                WalletModel walletModel = new WalletModel()
                {
                    UserId = orderSummary.ApplicationUserId,
                    WalletBalance = orderSummary.OrderTotal,
                    OrderId = orderSummary.Id

                };

                
                _unitOfWork.Wallet.Add(walletModel);
                _unitOfWork.Save();


                _unitOfWork.OrderSummary.UpdateStatus(orderSummary.Id, SD.StatusCancelled, SD.StatusRefunded);

            }

            else
            {
                _unitOfWork.OrderSummary.UpdateStatus(orderSummary.Id, SD.StatusCancelled, SD.StatusCancelled);

            }
            

            _unitOfWork.Save();
            TempData["Success"] = "Order  Cancelled Successfully";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderSummary.Id });


        }



        [ActionName("Details")]
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin )]
       
        public IActionResult PayNow()
        {
            var orderSummary = _unitOfWork.OrderSummary.Get(u => u.Id == OrderVM.OrderSummary.Id);

            orderSummary.PaymentDate = DateTime.Now;

            _unitOfWork.OrderSummary.UpdateStatus(orderSummary.Id, SD.StatusShipped, SD.PaymentStatusCompleted);
            _unitOfWork.Save();
            TempData["Success"] = "COD Payment Completed Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderSummary.Id });
        }


        [HttpGet]

        [Authorize(Roles = SD.Role_User)]
        public ActionResult GeneratePdf(int orderId)
        {


            OrderVM = new()
            {
                OrderSummary = _unitOfWork.OrderSummary.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderSummaryId == orderId, includeProperties: "Product")

            };

            var document = new PdfDocument();
             
               string htmlcontent = "<div style ='width:100%;text-align:center'>";
            htmlcontent += "<h2> NainaBoutique </h2>";
            

            if(OrderVM != null)
            {
                htmlcontent += "<h2> Invoice No: INV" + OrderVM.OrderSummary.Id  + "& Invoice Date:" + DateTime.Now + "</h2>";
                htmlcontent += "<h3> Customer: " + OrderVM.OrderSummary.Name + " " + "</h3>";
                htmlcontent += "<p>" + OrderVM.OrderSummary.Address + "," + OrderVM.OrderSummary.City + "</p>";
                htmlcontent += "<p>" + OrderVM.OrderSummary.State + "," + OrderVM.OrderSummary.PostalCode + "</p>";
                htmlcontent += "<h3> Contact :" + OrderVM.OrderSummary.MobileNumber +"</h3>";
                htmlcontent += "</div>";
            }


            htmlcontent += "<table style = 'width:100% ;border:1px solid #000'>";
            htmlcontent +=  "<thead style='font-weight :bold'>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style = 'border:1px solid #000'> Product Code </td>";
            htmlcontent += "<td style = 'border:1px solid #000'> Product Name </td>";
            htmlcontent += "<td style = 'border:1px solid #000'> Quantity</td>";
            htmlcontent += "<td style = 'border:1px solid #000'> Price </td>";
            htmlcontent += "<td style = 'border:1px solid #000'> Total Amount </td>";
            htmlcontent += "</tr>";
            htmlcontent += "</thead >";

            htmlcontent += "<tbody>";
            if(OrderVM != null)
            {
                foreach(var product in OrderVM.OrderDetail)
                {
                    htmlcontent += "<tr>";
                    htmlcontent += "<td>" + product.ProductId + "</td>";
                    htmlcontent += "<td>" + product.Product.ProductName+ "</td>";
                    htmlcontent += "<td>" + product.Count + "</td>";
                    htmlcontent += "<td>" + product.Price.ToString() +"AED "+ "</td>";
                    htmlcontent += "<td>" + ( product.Count * product.Price).ToString()+ "AED " + "</td>";
                    htmlcontent += "</tr>";
                };
            }

            htmlcontent += "</tbody>";
            htmlcontent += "</div>";
            htmlcontent += "<br/>";
            htmlcontent += "<br/>";
            htmlcontent += "<div style='text-align:left>";
            htmlcontent += "<table style = 'width:100% ;border:1px solid #000;float:right'>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style = 'border:1px solid #000'> Total Amount </td>";
            htmlcontent += "</tr>";


            if(OrderVM != null)
            {
                htmlcontent += "<tr>";
                htmlcontent += "<td style='border:1px solid #000'>" + OrderVM.OrderSummary.OrderTotal.ToString() + "AED" + "</td>";
                htmlcontent += "</tr>";
            }
            htmlcontent += "</table >";
            htmlcontent += "</div>";
            htmlcontent += "</div>";

            PdfGenerator.AddPdfPages(document, htmlcontent, PageSize.A4);
            byte[]? response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }

            string Filename = "Invoice_" + orderId + ".pdf";
            return File(response, "application/pdf", Filename);
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

