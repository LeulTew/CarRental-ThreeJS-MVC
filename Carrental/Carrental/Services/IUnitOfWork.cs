namespace Carrental.Services
{
	public interface IUnitOfWork
	{
		IPaypalServices PaypalServices { get; }
	}
}
