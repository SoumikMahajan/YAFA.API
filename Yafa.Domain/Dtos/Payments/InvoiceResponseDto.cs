namespace Yafa.Domain.Dtos.Payments;

public class InvoiceResponseDto
{
	public string InvoiceNumber { get; set; } = string.Empty;
	public DateTime IssuedDate { get; set; }

	public string CompanyName { get; set; } = string.Empty;
	public string? CompanyAddress { get; set; }
	public string CompanyTRN { get; set; } = string.Empty;
	public string? CompanyEmail { get; set; }
	public string? CompanyPhone { get; set; }

	public Guid BookingPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;
	public string GuestName { get; set; } = string.Empty;
	public string GuestEmail { get; set; } = string.Empty;
	public string? GuestPhone { get; set; }
	public string? GuestCountry { get; set; }

	public string PropertyName { get; set; } = string.Empty;
	public string? PropertyAddress { get; set; }
	public string? City { get; set; }
	public string? Country { get; set; }

	public DateOnly CheckInDate { get; set; }
	public DateOnly CheckOutDate { get; set; }
	public int TotalNights { get; set; }
	public int TotalGuests { get; set; }
	public decimal PricePerNight { get; set; }
	public decimal SubtotalAmount { get; set; }
	public decimal DiscountAmount { get; set; }
	public decimal TotalAmount { get; set; }
	public string PaymentPlanType { get; set; } = "Full";
	public decimal AmountPaid { get; set; }
	public decimal RemainingBalance { get; set; }

	// VAT shown separately, as UAE FTA tax invoices require
	public decimal VATAmount { get; set; }
	public decimal VATRate { get; set; }
	public decimal AmountExclVAT { get; set; }

	public List<InvoiceLineItemDto> LineItems { get; set; } = new();
	public List<InvoicePaymentDto> Payments { get; set; } = new();
}


public class InvoiceLineItemDto
{
	public string FeeName { get; set; } = string.Empty;
	public string FeeCategory { get; set; } = string.Empty;
	public decimal FeeValue { get; set; }
	public decimal CalculatedAmount { get; set; }
	public bool IsExtra { get; set; }
	public bool IsVATLine { get; set; }
}

public class InvoicePaymentDto
{
	public string PaymentType { get; set; } = string.Empty;   // "Full" | "Deposit" | "Balance" | "Extension"
	public decimal Amount { get; set; }
	public string CurrencyCode { get; set; } = string.Empty;
	public DateTime? PaidDate { get; set; }
	public string? CardBrand { get; set; }
	public string? CardLast4 { get; set; }
}
