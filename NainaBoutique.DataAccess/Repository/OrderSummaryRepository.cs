using System;
using NainaBoutique.DataAccess.Data;
using NainaBoutique.DataAccess.Repository.IRepository;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository
{
    public class OrderSummaryRepository : Repository<OrderSummary>, IOrderSummaryRepository
    {
        private ApplicationDbContext _db;

        public OrderSummaryRepository(ApplicationDbContext db) : base(db)
        {
           _db = db;
}

    public void Update(OrderSummary orderSummary)
            {
            _db.OrderSummaries.Update(orderSummary);
            }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _db.OrderSummaries.FirstOrDefault(u => u.Id == id);

            if(orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;

                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }

            }

        }

        public void UpdateStripePayment(int id, string sessionId, string paymentIntendId)
        {
            var orderFromDb = _db.OrderSummaries.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb!.SessionId = sessionId;


                
            }
            if (!string.IsNullOrEmpty(paymentIntendId))
            {
                orderFromDb!.PaymentIntendId = paymentIntendId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
        }
    }
        
}

