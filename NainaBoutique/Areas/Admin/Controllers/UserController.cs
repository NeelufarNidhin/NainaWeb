using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;
using NainaBoutique.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NainaBoutique.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork,ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;

        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            List<ApplicationUser> userList = _unitOfWork.User.GetAll().ToList();
            return View(userList);
        }
        public IActionResult Create()
        {
            return View();
        }
        

       

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _unitOfWork.User.GetAll().ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }

            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult BlockUnblock([FromBody] string id)
        {

            var userFromDb = _unitOfWork.User.Get(u => u.Id == id);
            if (userFromDb == null)
            {
                return Json(new { success = false, message = "Error while Opertaion" });
            }
           if(userFromDb.LockoutEnd!= null && userFromDb.LockoutEnd > DateTime.Now)
            {
                //unlock if locked
                userFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _unitOfWork.Save();

            return Json(new { success = true, message = "Operation Successfull" });
        }
        #endregion

    }
}