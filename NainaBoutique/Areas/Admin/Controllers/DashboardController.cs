using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;
using NainaBoutique.Models.ViewModels;
using NainaBoutique.Utility;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {

        public readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ReportVM? ReportVM { get; set; }

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /<controller>/
        public  async Task<IActionResult> Index()
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



        public IActionResult Chartjs()
        {
            return View();
        }

        [HttpPost]

        [Authorize(Roles = SD.Role_Admin)]
        public ActionResult GeneratePdf(DateTime StartDate , DateTime EndDate)
        {


            ReportVM = new()
            {
                OrderSummary = _unitOfWork.OrderSummary.GetAll(u => u.OrderDate >= StartDate && u.OrderDate <= EndDate, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderSummary.OrderDate >= StartDate && u.OrderSummary.OrderDate <= EndDate, includeProperties: "Product")

            };

            var document = new PdfDocument();

            string htmlcontent = "<div style ='width:100%;text-align:center'>";
            htmlcontent += "<h2> NainaBoutique </h2>";


            if (ReportVM != null)
            {
                htmlcontent += "<h2> Sales Report - From :" + StartDate + " To :" + EndDate + "</h2>";
                htmlcontent += "<h3> Report By Admin </h3>";
                htmlcontent += "<div>";
            }


            htmlcontent += "<table style ='width:100%; border: 1px solid #000'>";
            htmlcontent += "<thead style='font-weight:bold'>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style='border:1px solid #000'> Order Id </td>";
            htmlcontent += "<td style='border:1px solid #000'> Billing Name </td>";
            htmlcontent += "<td style='border:1px solid #000'>Order Date</td>";
            htmlcontent += "<td style='border:1px solid #000'>Total</td >";
            htmlcontent += "<td style='border:1px solid #000'>Order Status</td>";
            htmlcontent += "</tr>";
            htmlcontent += "</thead >";

            htmlcontent += "<tbody>";
            if (ReportVM != null)
            {
                foreach (var order in ReportVM.OrderSummary)
                {
                    htmlcontent += "<tr>";
                    htmlcontent += "<td>" + order.Id + "</td>";
                    htmlcontent += "<td>" + order.Name + "</td>";
                    htmlcontent += "<td>" + order.OrderDate + "</td >";
                    htmlcontent += "<td>" + order.OrderTotal.ToString("C", new CultureInfo("en-US")) + "</td>";
                    htmlcontent += "<td>" + order.OrderStatus + "</td >";
                    htmlcontent += "</tr>";
                };
            }

            htmlcontent += "</tbody>";
            
            htmlcontent += "</table >";
            htmlcontent += "</div>";
            htmlcontent += "</div>";
            htmlcontent += "</div>";

            PdfGenerator.AddPdfPages(document, htmlcontent, PageSize.A4);
            byte[]? response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }

            string Filename = "SalesReport-" + StartDate + ".pdf";
            return File(response, "application/pdf", Filename);
        }

        [HttpGet]

        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult ExportExcel()
        {

            var orderData = GetSalesReport();
            using(XLWorkbook wb = new XLWorkbook())
            {
                wb.AddWorksheet(orderData, "Sales Records");

                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "Sample.xlsx");
                }
            }
        }


        public DataTable GetSalesReport()
        {
            DataTable dt = new DataTable();
            dt.TableName = "SalesReport";
            dt.Columns.Add(" Id", typeof(int));
            dt.Columns.Add(" Name", typeof(string));
            dt.Columns.Add(" OrderDate", typeof(DateTime));
            dt.Columns.Add(" OrderTotal", typeof(float));
            dt.Columns.Add(" OrderStatus", typeof(string));


            var OrderList = _unitOfWork.OrderSummary.GetAll().ToList();

            if(OrderList.Count > 0)
            {
                OrderList.ForEach(item =>
                {
                    dt.Rows.Add(item.Id, item.Name, item.OrderDate, item.OrderTotal, item.OrderStatus);
                });
            }
            return dt;
        }

    }
}

