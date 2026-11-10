namespace Yafa.Domain.Dtos.Payments;

public class StripeSettings
{
	public string PublishableKey { get; set; } = string.Empty;

	public string SecretKey { get; set; } = string.Empty;

	public string WebhookSecret { get; set; } = string.Empty;
}

public class StripeInitiatePaymentResponseDto
{
	public Guid BookingPublicId { get; set; }
	public Guid PaymentPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public string CurrencyCode { get; set; } = "INR";
	public string ClientSecret { get; set; } = string.Empty;
	public string PaymentIntentId { get; set; } = string.Empty;
	public string PaymentPlanType { get; set; } = "Full";
	public decimal BookingTotalAmount { get; set; }
	public decimal BookingAmountPaid { get; set; }
}

public class ProcessStripeWebhookRequestDTO
{
	public string StripeEventId { get; set; } = string.Empty;
	public string PaymentIntentId { get; set; } = string.Empty;
	public string EventType { get; set; } = string.Empty;
	public string PaymentIntentStatus { get; set; } = string.Empty;  // NEW: e.g. "succeeded", "processing", "requires_action"
	public string CustomerId { get; set; } = string.Empty;
	public string PaymentMethodId { get; set; } = string.Empty;
	public string ChargeId { get; set; } = string.Empty;

	public string Payload { get; set; } = string.Empty;
	public string CardBrand { get; set; } = string.Empty;
	public string CardLast4 { get; set; } = string.Empty;
}