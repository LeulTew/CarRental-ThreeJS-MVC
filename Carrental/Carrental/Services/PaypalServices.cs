using PayPal.Api;
namespace Carrental.Services
{
	public class PaypalServices : IPaypalServices
	{
		private readonly APIContext apiContext;
		private readonly Payment payment;
		private readonly IConfiguration configuration;
        public PaypalServices(IConfiguration configuration)
        {
            this.configuration = configuration;

            try
            {
                var clientId = configuration["PayPal:ClientId"];
                var clientSecret = configuration["PayPal:ClientSecret"];

                var config = new Dictionary<string, string>
            {
                { "mode", "sandbox" }, // Use "sandbox for testing, "live" for production
                {"clientId", clientId},
                {"clientSecret", clientSecret}
            };

                var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
                apiContext = new APIContext(accessToken);

                payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" }
                };
            }
            catch (Exception ex)
            {
                // Handle the exception gracefully
                // For example, log the error and provide default values for apiContext and payment
                Console.WriteLine("Error accessing PayPal: " + ex.Message);
                apiContext = null; // Set default value for apiContext
                payment = null; // Set default value for payment
            }
        }

        public async Task<Payment> CreateOrderAsync(decimal TotalAmount, string returnUrl, string cancelUrl)
		{
			var apiContext = new APIContext(new OAuthTokenCredential(configuration["PayPal:ClientId"], configuration["PayPal:ClientSecret"]).GetAccessToken());
			
			var itemList = new ItemList()
			{
				items = new List<Item>()
				{
					new Item()
					{
						name = "Car Rent Fee",
						currency = "USD",
						price = TotalAmount.ToString("0.00"),
						quantity = "1",
						sku = "Rental"
					}
				}
			};

			var transaction = new Transaction()
			{
				amount = new Amount()
				{
					currency = "USD",
					total = TotalAmount.ToString("0.00"),
					details = new Details()
					{
						subtotal = TotalAmount.ToString("0.00")
					}
				},
				item_list = itemList,
				description = "Car Rent Fee"
			};

			var payment = new Payment()
			{
				intent = "sale",
				payer = new Payer() { payment_method = "paypal" },
				redirect_urls = new RedirectUrls()
				{
					return_url = returnUrl,
					cancel_url = cancelUrl
				},
				transactions = new List<Transaction>() { transaction }
			};

			var createdPayment = payment.Create(apiContext);
			return createdPayment;
		}

	}
}
