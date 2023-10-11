using System;
using System.Linq.Expressions;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository
{
    public class AddressRepository : Repository<AddressModel>, IAddressRepository
    {
        private ApplicationDbContext _db;

        public AddressRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

       

        public void Update(AddressModel address)
        {
            _db.Address.Update(address);
        }
    }
}

