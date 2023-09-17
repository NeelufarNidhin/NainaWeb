using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Models.Models;
using NainaBoutique.Models.ViewModels;
using NainaBoutique.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{
    [Area("admin")]
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
            OrderVM = new() { 
           

                OrderSummary = _unitOfWork.OrderSummary.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderSummaryId == orderId, includeProperties: "Product")
            };

            return View(OrderVM);
        }


        #region API CALLS


        [HttpGet]
        public IActionResult GetAll(string status )
        {
            IEnumerable<OrderSummary> objOrderSummaryList = _unitOfWork.OrderSummary.GetAll(includeProperties: "ApplicationUser").ToList();
           

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

