using QuestPDF.Fluent;
using Yafa.Application.Common.Interfaces;
using Yafa.Domain.Dtos.Payments;

namespace Yafa.Infrastructure.Repository
{
	public class InvoicePdfService : IInvoicePdfService
	{
		public async Task<byte[]> GenerateInvoicePdfAsync(InvoiceResponseDto invoice)
		{
			ArgumentNullException.ThrowIfNull(invoice);

			var document = new InvoicePdfDocument(invoice);

			return document.GeneratePdf();
		}
	}
}
