using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.Models;
using NainaBoutique.Utility;

namespace NainaBoutique.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

       public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager , ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public void Initialize()
        {

            //migrations if not applied
            try
            {
                if( _db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (!_roleManager.RoleExistsAsync(SD.Role_User).GetAwaiter().GetResult())
            {

                _roleManager.CreateAsync(new IdentityRole(SD.Role_User)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "nilu.nidhin@gmail.com",
                    Email = "nilu.nidhin@gmail.com",
                    Name = "Neelufar Nidhin",
                    Address = "Fathima Building",
                    City = "Abudhabi",
                    State = "Abudhabi",
                    PostalCode = 3245,
                    MobileNumber = 989345556
                }, "User!1234").GetAwaiter().GetResult();

                ApplicationUser? user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "nilu.nidhin@gmail.com");
                _userManager.AddToRoleAsync(user!, SD.Role_Admin).GetAwaiter().GetResult();
            }


            return;

        }
       
    }
}

