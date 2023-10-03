using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;

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
        public List<object> GetSalesData()
        {
            List<object> data = new List<object>();

            List<string> labels2 = _db.OrderDetails.Select(u => u.Product.ProductName.ToString()).ToList();
            data.Add(labels2);

            List<string> labels = _db.OrderSummaries.Select(u => u.OrderTotal.ToString()).ToList();
            data.Add(labels);

           

            return data;
            
        }
    }
}

