namespace Yafa.Domain.Dtos.Payments
{
	public class PaymentResultDto
	{
		// Payment slot (BookingPayments)
		public Guid PaymentPublicId { get; set; }
		public string PaymentType { get; set; } = string.Empty;    // "Full" | "Deposit" | "Balance"
		public string PaymentGateway { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public string CurrencyCode { get; set; } = string.Empty;
		public string PaymentStatus { get; set; } = string.Empty;  // Pending | Succeeded | Failed | Cancelled
		public DateTime? InitiatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }

		// Stripe detail
		public string? PaymentIntentId { get; set; }
		public string? ChargeId { get; set; }
		public string? StripeStatus { get; set; }
		public string? CardBrand { get; set; }
		public string? CardLast4 { get; set; }


		// Just enough booking context to label the receipt
		public Guid BookingPublicId { get; set; }
		public string BookingReference { get; set; } = string.Empty;
		public string PaymentPlanType { get; set; } = string.Empty;
		public string PropertyName { get; set; } = string.Empty;
		public DateOnly CheckInDate { get; set; }

		// Only meaningful when this was a Deposit and it succeeded
		public decimal RemainingBalance { get; set; }
	}
}
