using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NainaBoutique.Models;


namespace NainaBoutique.DataAccess.Data;

public class ApplicationDbContext : DbContext 
{
    

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public ApplicationDbContext()
    {

    }


    public DbSet<CategoryModel> Categories { get; set; }

    public DbSet<ProductModel> Products { get; set; }
}

