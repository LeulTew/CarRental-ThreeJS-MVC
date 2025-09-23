using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Carrental.Models
{
    public class PaymentViewModel
    {
        public int BookingId { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionId { get; set; }
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Card Number is required")]
        public string? CardNumber { get; set; }

        [Required(ErrorMessage = "Expiry Date is required")]
        public string? ExpiryDate { get; set; }

        [Required(ErrorMessage = "CVV is required")]
        public string? CVV { get; set; }
        [Required(ErrorMessage = "Please select the payment method")]
        public string? PaymentMethod { get; set; }
    }

}
