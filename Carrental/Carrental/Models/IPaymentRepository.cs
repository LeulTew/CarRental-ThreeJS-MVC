namespace Carrental.Models
{
    public interface IPaymentRepository
    {
        void AddPayment(Payment payment);
        void DeletePayment(string transactionId);
    }

}
