using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository;
using NainaBoutique.DataAccess.Repository.IRepository;
//using NainaBoutique.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using NainaBoutique.Utility;
using static NainaBoutique.Areas.Customer.Controllers.HomeController;
using NainaBoutique.Areas.Identity.Pages.Account;
using Stripe;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using NainaBoutique.DataAccess.DbInitializer;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton<OtpService>();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LoginPath = $"/Identity/Account/Logout";
    options.LoginPath = $"/Identity/Account/AccessDenied";

});
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
var app = builder.Build();


app.UseMiddleware<ErrorHandlingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

}
else
{
    
    app.UseExceptionHandler(
        options =>
        {
            options.Run(
                async (context) =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        await context.Response.WriteAsync(ex.Error.Message);
                    }
                });
        });


}



//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("Home/Error");
//}
app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
SeedDatabase();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


 app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope()) { 
    
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}