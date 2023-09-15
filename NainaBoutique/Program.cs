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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSingleton<OtpService>();

builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LoginPath = $"/Identity/Account/Logout";
    options.LoginPath = $"/Identity/Account/AccessDenied";

});
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<ErrorHandlingMiddleware>();
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
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   // app.UseHsts();
}



//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("Home/Error");
//}
app.UseHttpsRedirection();
app.UseStaticFiles();
//StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

