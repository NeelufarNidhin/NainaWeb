using System;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;

namespace NainaBoutique.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork

    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category  { get; private set;}

       
        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Category = new CategoryRepository(_db);
        }
        
        public void Save()
        {
            _db.SaveChanges();
        }

        
    }
}

