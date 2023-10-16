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
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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

            //stock management after shipping the order
            var orderDetails = _unitOfWork.OrderDetail.Get(u => u.OrderSummaryId == orderSummary.Id);

            var productFromDb = _unitOfWork.Product.Get(u => u.Id == orderDetails.ProductId);

            if(productFromDb != null)
            {
                productFromDb.QuantityInStock = productFromDb.QuantityInStock - orderDetails.Count;
                _unitOfWork.Product.Update(productFromDb);
                _unitOfWork.Save();
            }
            
            orderSummary.TrackingNumber = OrderVM.OrderSummary.TrackingNumber;
            orderSummary.Carrier = OrderVM.OrderSummary.Carrier;
            orderSummary.OrderStatus = SD.StatusShipped;
            orderSummary.ShippingDate = DateTime.Now;

            if(orderSummary.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderSummary.PaymentDueDate = DateTime.Now.AddDays(14);
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
            var orderDetail = _unitOfWork.OrderDetail.Get(u => u.OrderSummaryId == orderSummary.Id);
            var productFromDb = _unitOfWork.Product.Get(u => u.Id ==orderDetail.ProductId);

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


       


       // PDF GENERATOR //

        [HttpGet]

        [Authorize(Roles = SD.Role_User)]
        public FileStreamResult GetPDF(int orderId)
        {

            
            OrderVM = new()
            {
                OrderSummary = _unitOfWork.OrderSummary.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderSummaryId == orderId, includeProperties: "Product")

            };

            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header()
                        
                        .Row(row =>
                        {

                            
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().AlignCenter().Text("NAINA BOUTIQUE").Bold().FontSize(30);
                                column.Item().Text(" ");
                                column.Item().Text(" ");
                                column.Item().Text($"Invoice #{OrderVM.OrderSummary.Id} ");

                                column.Item().Text(text =>
                            {
                                text.Span("OrderDate: ").SemiBold();
                                text.Span($"{OrderVM.OrderSummary.OrderDate}");
                            });

                                column.Item().Text(text =>
                                {
                                    text.Span("Name :").SemiBold();
                                    text.Span($"{OrderVM.OrderSummary.Name}");
                                });

                                column.Item().Text(text =>
                                {
                                    text.Span("Address: ").SemiBold();
                                    text.Span($"{OrderVM.OrderSummary.Address + ", " + OrderVM.OrderSummary.City}");
                                });
                                column.Item().Text(text =>
                                {
                                    text.Span("State: ").SemiBold();
                                    text.Span($" {OrderVM.OrderSummary.State + "," + OrderVM.OrderSummary.PostalCode}");
                                });
                                column.Item().Text(text =>
                                {
                                    text.Span("MobileNumber: ").SemiBold();
                                    text.Span($"{OrderVM.OrderSummary.MobileNumber}");
                                });
                            });
                        });

                        page.Content()
                        .Height(250)
                        .AlignCenter()
                        .AlignMiddle()
                        //.BorderBottom(1)
                        .PaddingVertical(4).Table(table =>
                        {

                            table.ColumnsDefinition(columns =>
                            {
                                // s.no, name,qty, price, total
                                columns.ConstantColumn(25);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            table.Header
                            (header =>
                            {
                                header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Text("#Id");
                                header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Text("Name");
                                header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text("Quantity");
                                header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text("Price");
                                header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text("Total");

                            });

                           

                            if (OrderVM != null)
                            {
                                foreach (var product in OrderVM.OrderDetail)
                                {
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Text(product.ProductId.ToString());
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Text(product.Product!.ProductName);
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text(product.Count.ToString());
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text(product.Price.ToString("F2") + " AED");
                                    table.Cell().Border(1).BorderColor( Colors.Grey.Lighten2).AlignRight().Text((product.Count * product.Price).ToString("F2") + " AED");
                                    

                                }
                            }

                            table.Cell().Text(" ");
                            table.Cell().Text(" ");
                            table.Cell().Text(" ");
                            table.Cell().AlignRight().Text("Grand total:");
                            table.Cell().AlignRight().Text($"{OrderVM!.OrderSummary.OrderTotal.ToString("F2")}" + "AED");
                                
                           


                        });

                    

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });
            byte[] pdfBytes = document.GeneratePdf();
            MemoryStream ms = new MemoryStream(pdfBytes);
            return new FileStreamResult(ms, "application/pdf");

        }

        //API CALL TO  GET ALL THE ORDERS ///

        #region API CALLS
       

        [HttpGet]
        public IActionResult GetAll(string status )
        {
            
            IEnumerable<OrderSummary> objOrderSummaryList;


            

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

