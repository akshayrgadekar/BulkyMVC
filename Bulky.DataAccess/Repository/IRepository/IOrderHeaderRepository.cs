using Bulky.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository:IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
        void UpdateStatus(int orderId, string orderStatus, string? paymentStaus = null);

        void UpdateStripePaymentId(int orderId, string sessionId, string PaymentIntentId);
    }
}
