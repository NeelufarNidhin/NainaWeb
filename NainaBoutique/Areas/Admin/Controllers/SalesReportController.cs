using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;
using NainaBoutique.Models.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SalesReportController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly ApplicationDbContext _db;

        public SalesReportController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public List<object> GetSalesData(  DateTime fromDate, DateTime toDate)

        {

            //fromDate = DateTime.Now.AddDays(-10);

            //toDate = DateTime.UtcNow;
            List<object> data = new List<object>();

            var labels2 = _unitOfWork.OrderSummary.GetAll(u => u.OrderDate >= fromDate && u.OrderDate <= toDate, includeProperties :"ApplicationUser");
                
            data.Add(labels2);

            var labels = _unitOfWork.OrderDetail.GetAll(u => u.OrderSummary.OrderDate >= fromDate && u.OrderSummary.OrderDate <= toDate, includeProperties: "Product");
            data.Add(labels);

           

            return data;
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

            //// Convert the sales data to labels and data for the chart
            //var labels = salesData.Select(item => item.Date.ToShortDateString()).ToList();
            //var data = salesData.Select(item => item.TotalSales).ToList();

            //ViewBag.ChartLabels = labels;
            //ViewBag.ChartData = data;

            return View();
        }




        //public ActionResult MonthlySalesByDate(string _year, string _month)
        //{
        //    //assign incoming values to the variables
        //    int year = 0, month = 0;
        //    //check if year is null
        //    if (string.IsNullOrWhiteSpace(_year) && _month != null)
        //    {
        //        year = DateTime.Now.Date.Year;
        //        month = Convert.ToInt32(_month.Trim());
        //    }
        //    else
        //    {
        //        year = Convert.ToInt32(_year.Trim());
        //        month = Convert.ToInt32(_month.Trim());
        //    }
        //    //calculate ttal number of days in a particular month for a that year 
        //    int daysInMonth = DateTime.DaysInMonth(year, month);
        //    var days = Enumerable.Range(1, daysInMonth);
        //    var query = _db.OrderSummaries.Where(x => x.OrderDate.Year == year && x.OrderDate.Month == month).OrderBy(x => x.OrderDate).Select(g => new
        //    {
        //        Day = g.OrderDate.Day,
        //        Total = g.OrderTotal
        //    });
        //    //var model = new OrderVM
        //    //{
        //    //    OrderDate = new DateTime(year, month, 1),
        //    //    Days = days.GroupJoin(query, d => d, q => q.Day, (d, q) => new DayTotalVM
        //    //    {
        //    //        Day = d,
        //    //        Total = q.Sum(x => x.Total)
        //    //    }).ToList()
        //    //};
        //    // return View(model);

        //    return View();
        //}

        //[HttpPost]
        //public IActionResult GenerateReport(string reportType, DateTime fromDate, DateTime toDate)
        //{
        //    List<OrderSummary> filteredSalesData = null;

        //    if (reportType == "Daily")
        //    {
        //        filteredSalesData = _db.OrderSummaries
        //            .Where(s => s.OrderDate.Date == selectedDate.Date)
        //            .OrderBy(s => s.SaleDate)
        //            .ToList();
        //    }
        //    else if (reportType == "Weekly")
        //    {
        //        filteredSalesData = _db.OrderSummaries
        //        .Where(s => s.OrderDate >= fromDate && s.OrderDate <= toDate)
        //        .OrderBy(s => s.SaleDate)
        //        .ToList();
        //    }
        //    else if (reportType == "Monthly")
        //    {
        //        filteredSalesData = _db.OrderSummaries
        //            .Where(s => s.SaleDate.Month == selectedDate.Month && s.SaleDate.Year == selectedDate.Year)
        //            .OrderBy(s => s.SaleDate)
        //            .ToList();
        //    }

        //    return View("Index", filteredSalesData);
        //}
        

    }
}

