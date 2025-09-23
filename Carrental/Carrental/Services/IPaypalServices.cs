using PayPal.Api;
namespace Carrental.Services
{
	public interface IPaypalServices
	{
		Task<Payment> CreateOrderAsync(decimal TotalAmount, string returnUrl, string cancelUrl);
	}
}
