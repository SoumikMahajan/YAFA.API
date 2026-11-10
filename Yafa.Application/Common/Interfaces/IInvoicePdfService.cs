using Yafa.Domain.Dtos.Payments;

namespace Yafa.Application.Common.Interfaces
{
	public interface IInvoicePdfService
	{
		Task<byte[]> GenerateInvoicePdfAsync(InvoiceResponseDto invoice);
	}
}
