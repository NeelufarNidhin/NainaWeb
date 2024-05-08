using System;
using NainaBoutique.Models.Models;

namespace NainaBoutique.DataAccess.Repository.IRepository
{
	public interface IOrderSummaryRepository : IRepository<OrderSummary>
	{
		void Update(OrderSummary orderSummary);
		void UpdateStatus(int id, string OrderStatus, string PaymentStatus = null);
		void UpdateStripePayment(int id, string SessionId, string PaymentIntendId);
	}
}

