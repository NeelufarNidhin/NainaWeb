using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
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
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public readonly ApplicationDbContext _db;
        public readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ReportVM? ReportVM { get; set; }
        public ProductViewModel cc { get; set; }

        public DashboardController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            IEnumerable<OrderSummary> orderSummary = _unitOfWork.OrderSummary.GetAll().ToList();

            int shippedCount = orderSummary.Count(u => u.OrderStatus == "Shipped");
            int approvedCount = orderSummary.Count(u => u.OrderStatus == "Approved");
            int cancelledCount = orderSummary.Count(u => u.OrderStatus == "Cancelled");
            int pendingCount = orderSummary.Count(u => u.OrderStatus == "Pending");



            int totalOrders = orderSummary.Count();

            ViewBag.ShippedCount = shippedCount;
            ViewBag.ApprovedCount = approvedCount;
            ViewBag.CancelledCount = cancelledCount;
            ViewBag.PendingCount = pendingCount;

            ViewBag.TotalOrders = totalOrders;

            


            return View();
        }

        public IActionResult SalesChart(DateTime startDate, DateTime endDate)
        {
            // Query the database to get sales data within the date range
            var salesData = _db.OrderSummaries
                .Where(order => order.OrderDate >= startDate && order.OrderDate <= endDate)
                .GroupBy(order => order.OrderDate.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    TotalSales = group.Sum(order => order.OrderTotal)
                })
                .OrderBy(item => item.Date)
                .ToList();

            // Convert the sales data to labels and data for the chart
            var labels = salesData.Select(item => item.Date.ToShortDateString()).ToList();
            var data = salesData.Select(item => item.TotalSales).ToList();

            ViewBag.ChartLabels = labels;
            ViewBag.ChartData = data;

            return View();
        }





        
        

        [HttpGet]

        //EXPORTING
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult ExportExcel(DateTime StartDate, DateTime EndDate)
        {

            var orderData = GetSalesReport(StartDate,EndDate);
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.AddWorksheet(orderData, "Sales Records");

                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "Sample.xlsx");
                }
            }
        }

        
        //EXCEL GENERATOR ( ORDER DETAILS)
        public DataTable GetSalesReport(DateTime StartDate, DateTime EndDate)
        {
            ReportVM = new()
            {
                OrderSummary = _unitOfWork.OrderSummary.GetAll(u => u.OrderDate >= StartDate && u.OrderDate <= EndDate, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderSummary.OrderDate >= StartDate && u.OrderSummary.OrderDate <= EndDate, includeProperties: "Product")

            };

            DataTable dt = new DataTable();
            dt.TableName = "SalesReport";
            dt.Columns.Add(" Id", typeof(int));
            dt.Columns.Add(" Name", typeof(string));
            dt.Columns.Add(" Quantity", typeof(string));
            dt.Columns.Add(" Price", typeof(float));
            dt.Columns.Add(" TotalPrice", typeof(float));


            var OrderDetailList = _db.OrderDetails.Where(u => u.OrderSummary.OrderDate >= StartDate && u.OrderSummary.OrderDate <= EndDate).ToList();

            var salesReport = OrderDetailList
          .GroupBy(detail => new { detail.ProductId, detail.Product.ProductName, detail.Count, detail.Price })
          .Select(group => new
          {
              ProductId = group.Key.ProductId,
              Name = group.Key.ProductName,
              Count = group.Key.Count,
              Price = group.Key.Price

          })
          .ToList();

            if (ReportVM != null)
            {
              
                    salesReport.ForEach(item =>
                {
                    dt.Rows.Add(item.ProductId, item.Name, item.Count, item.Price, (item.Count * item.Price));
                });
            }
            return dt;
        }



        //ORDER SUMMARY
        public DataTable GetSalesReports()
        {
            DataTable dt = new DataTable();
            dt.TableName = "SalesReport";
            dt.Columns.Add(" Id", typeof(int));
            dt.Columns.Add(" Name", typeof(string));
            dt.Columns.Add(" OrderDate", typeof(DateTime));
            dt.Columns.Add(" OrderTotal", typeof(float));
            dt.Columns.Add(" OrderStatus", typeof(string));


            var OrderList = _unitOfWork.OrderSummary.GetAll().ToList();

            if (OrderList.Count > 0)
            {
                OrderList.ForEach(item =>
                {
                    dt.Rows.Add(item.Id, item.Name, item.OrderDate, item.OrderTotal, item.OrderStatus);
                });
            }
            return dt;
        }


        // PDF GENERATOR //

        [HttpGet]

        [Authorize(Roles = SD.Role_Admin)]
        public FileStreamResult GeneratePdf(DateTime StartDate, DateTime EndDate)
        {
            ReportVM = new()
            {
                OrderSummary = _unitOfWork.OrderSummary.GetAll(u => u.OrderDate >= StartDate && u.OrderDate <= EndDate, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderSummary.OrderDate >= StartDate && u.OrderSummary.OrderDate <= EndDate, includeProperties: "Product")

            };

            List<OrderDetail> orderDetails = _db.OrderDetails.Where(u => u.OrderSummary.OrderDate >= StartDate && u.OrderSummary.OrderDate <= EndDate).ToList();

            var salesReport = orderDetails
           .GroupBy(detail => new { detail.ProductId, detail.Product.ProductName,detail.Count ,detail.Price})
           .Select(group => new
           {
               ProductId = group.Key.ProductId,
               Name = group.Key.ProductName,
               Count = group.Key.Count,
               Price = group.Key.Price
              
           })
           .ToList();

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
                                column.Item().Text($"SalesReport-  From :\" + {StartDate}+ \" To :\" + {EndDate}+ # ");

                                
                                
                            });
                        });

                    page.Content()
                       .Height(250)
                       .AlignCenter()
                       .AlignMiddle()
                     
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



                           if (ReportVM != null)
                           {
                               foreach (var item in salesReport)
                               {
                                   table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Text(item.ProductId.ToString());
                                   table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Text(item.Name);
                                   table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text(item.Count.ToString());
                                   table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text(item.Price.ToString("F2") + " AED");
                                   table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text((item.Count * item.Price).ToString("F2") + " AED");
                                 




                               }
                           }

                           



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



        public IActionResult SalesReport(DateTime StartDate, DateTime EndDate)
        {
            // Retrieve order details from the database
            List<OrderDetail> orderDetails = _db.OrderDetails.Where(u => u.OrderSummary.OrderDate >= StartDate && u.OrderSummary.OrderDate <= EndDate).ToList();

            // Group order details by product and calculate total sales per product
            var salesReport = orderDetails
                .GroupBy(detail => new { detail.ProductId, detail.Product.ProductName })
                .Select(group => new
                {
                    ProductId = group.Key.ProductId,
                    Name = group.Key.ProductName,
                    TotalSales = group.Sum(detail => detail.Price)
                })
                .ToList();

            return View(salesReport);

        }

            #region API CALLS


            [HttpPost]
        public IActionResult GetAll( DateTime StartDate,DateTime EndDate)
        {

            ReportVM = new()
            {
                OrderSummary = _unitOfWork.OrderSummary.GetAll(u => u.OrderDate >= StartDate && u.OrderDate <= EndDate, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderSummary.OrderDate >= StartDate && u.OrderSummary.OrderDate <= EndDate, includeProperties: "Product")

            };


            return Json(new { data = ReportVM });
        }

        #endregion
    }

}