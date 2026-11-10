using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Domain.Dtos.Property;

public class CreateExtensionHoldRequest
{
	public Guid BookingPublicId { get; set; }
	public string UserId { get; set; } = string.Empty;
	public DateOnly NewCheckOutDate { get; set; }
}

public class ExtensionHoldResultDTO
{
	public Guid ExtensionHoldPublicId { get; set; }
	public Guid PropertyPublicId { get; set; }
	public string PropertyName { get; set; } = string.Empty;
	public string? PropertyAddress { get; set; }
	public int? TotalBedrooms { get; set; }
	public int? TotalBathrooms { get; set; }
	public int? MaxPersons { get; set; }

	public DateOnly CheckInDate { get; set; }
	public DateOnly CurrentCheckOutDate { get; set; }
	public DateOnly NewCheckOutDate { get; set; }
	public int ExtraNights { get; set; }
	public int NewTotalNights { get; set; }
	public decimal PricePerNight { get; set; }

	// No discount/coupon on an extension — deliberately no DiscountAmount field here.
	public decimal CurrentTotalAmount { get; set; }
	public decimal ExtraSubtotalAmount { get; set; }
	public decimal NewSubtotalAmount { get; set; }
	public decimal NewTotalAmount { get; set; }

	/// <summary>What the "Pay AED {ExtraAmount}" button charges: NewTotalAmount - CurrentTotalAmount.</summary>
	public decimal ExtraAmount { get; set; }

	public DateTime HoldExpiryTime { get; set; }
	public int SecondsRemaining { get; set; }

	public List<BookingFeeDTO>? FeeBreakdown { get; set; } = new();
}

public class InitiateExtensionPaymentRequest
{
	public Guid ExtensionHoldPublicId { get; set; }
	public string UserId { get; set; } = string.Empty;
}

public class ExtensionPaymentResponseDto
{
	public Guid BookingPublicId { get; set; }
	public Guid PaymentPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public string CurrencyCode { get; set; } = string.Empty;
	public string PaymentStatus { get; set; } = string.Empty;
	public Guid ExtensionHoldPublicId { get; set; }
	public DateOnly NewCheckOutDate { get; set; }
	public int SecondsRemaining { get; set; }
}

public class StripeInitiateExtensionPaymentResponseDto
{
	public Guid BookingPublicId { get; set; }
	public Guid PaymentPublicId { get; set; }
	public string BookingReference { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public string CurrencyCode { get; set; } = "INR";
	public string ClientSecret { get; set; } = string.Empty;
	public string PaymentIntentId { get; set; } = string.Empty;
	public Guid ExtensionHoldPublicId { get; set; }
	public DateOnly NewCheckOutDate { get; set; }
}
