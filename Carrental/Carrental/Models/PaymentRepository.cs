using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
namespace Carrental.Models
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly CarContext _context;

        public PaymentRepository(CarContext context)
        {
            _context = context;
        }

        public void AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            _context.SaveChanges();
        }
        public void DeletePayment(string transactionId)
        {
            var payment = _context.Payments.Where(p => p.TransactionId == transactionId).FirstOrDefault();
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                _context.SaveChanges();
            }
        }

    }

}
