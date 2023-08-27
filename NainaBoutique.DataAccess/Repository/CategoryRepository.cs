using System;
using System.Linq.Expressions;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models;

namespace NainaBoutique.DataAccess.Repository
{
    public class CategoryRepository : Repository<CategoryModel>, ICategoryRepository 
    {
        private ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

       
        public void Update(CategoryModel category)
        {
            _db.Categories.Update(category);
        }
    }
}

