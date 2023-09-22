using System;
using NainaBoutique.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IUserRepository : IRepository<ApplicationUser>
	{
        void Update(ApplicationUser applicationUser);
    }
}

