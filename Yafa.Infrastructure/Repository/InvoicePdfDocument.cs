using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Yafa.Domain.Dtos.Payments;

namespace Yafa.Infrastructure.Repository
{
	public class InvoicePdfDocument : IDocument
	{
		private readonly InvoiceResponseDto _invoice;

		public InvoicePdfDocument(InvoiceResponseDto invoice)
		{
			_invoice = invoice;
		}

		public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

		public void Compose(IDocumentContainer container)
		{
			container.Page(page =>
			{
				page.Size(PageSizes.A4);
				page.Margin(30);
				page.DefaultTextStyle(x => x.FontSize(10));

				page.Header().Element(ComposeHeader);
				page.Content().Element(ComposeContent);
				page.Footer().AlignCenter().Text(x =>
				{
					x.Span("Page ");
					x.CurrentPageNumber();
					x.Span(" of ");
					x.TotalPages();
				});
			});
		}

		private void ComposeHeader(IContainer container)
		{
			container.Row(row =>
			{
				row.RelativeItem().Column(col =>
				{
					col.Item().Text(_invoice.CompanyName).FontSize(16).Bold();
					if (!string.IsNullOrEmpty(_invoice.CompanyAddress))
						col.Item().Text(_invoice.CompanyAddress);
					col.Item().Text($"TRN: {_invoice.CompanyTRN}");
					if (!string.IsNullOrEmpty(_invoice.CompanyEmail))
						col.Item().Text(_invoice.CompanyEmail!);
					if (!string.IsNullOrEmpty(_invoice.CompanyPhone))
						col.Item().Text(_invoice.CompanyPhone!);
				});

				row.RelativeItem().Column(col =>
				{
					col.Item().AlignRight().Text("TAX INVOICE").FontSize(18).Bold();
					col.Item().AlignRight().Text($"Invoice No: {_invoice.InvoiceNumber}");
					col.Item().AlignRight().Text($"Issued: {_invoice.IssuedDate:dd MMM yyyy}");
					col.Item().AlignRight().Text($"Booking Ref: {_invoice.BookingReference}");
				});
			});
		}

		private void ComposeContent(IContainer container)
		{
			container.PaddingVertical(15).Column(col =>
			{
				col.Spacing(12);

				col.Item().Row(row =>
				{
					row.RelativeItem().Column(c =>
					{
						c.Item().Text("Billed To").Bold();
						c.Item().Text(_invoice.GuestName);
						c.Item().Text(_invoice.GuestEmail);
						if (!string.IsNullOrEmpty(_invoice.GuestPhone)) c.Item().Text(_invoice.GuestPhone!);
						if (!string.IsNullOrEmpty(_invoice.GuestCountry)) c.Item().Text(_invoice.GuestCountry!);
					});

					row.RelativeItem().Column(c =>
					{
						c.Item().Text("Property").Bold();
						c.Item().Text(_invoice.PropertyName);
						if (!string.IsNullOrEmpty(_invoice.PropertyAddress)) c.Item().Text(_invoice.PropertyAddress!);
						c.Item().Text($"{_invoice.City} {_invoice.Country}".Trim());
					});

					row.RelativeItem().Column(c =>
					{
						c.Item().Text("Stay Details").Bold();
						c.Item().Text($"Check-in: {_invoice.CheckInDate:dd MMM yyyy}");
						c.Item().Text($"Check-out: {_invoice.CheckOutDate:dd MMM yyyy}");
						c.Item().Text($"{_invoice.TotalNights} night(s), {_invoice.TotalGuests} guest(s)");
					});
				});

				col.Item().Element(ComposeLineItemsTable);
				col.Item().Element(ComposeTotals);

				if (_invoice.Payments.Any())
					col.Item().Element(ComposePaymentsTable);

				if (_invoice.RemainingBalance <= 0)
					col.Item().PaddingTop(10).Text("This invoice is paid in full.").FontColor(Colors.Green.Darken2).Bold();
			});
		}

		private void ComposeLineItemsTable(IContainer container)
		{
			container.Table(table =>
			{
				table.ColumnsDefinition(columns =>
				{
					columns.RelativeColumn(4);
					columns.RelativeColumn(2);
					columns.RelativeColumn(2);
					columns.RelativeColumn(2);
				});

				table.Header(header =>
				{
					header.Cell().Text("Description").Bold();
					header.Cell().AlignRight().Text("Category").Bold();
					header.Cell().AlignRight().Text("Rate").Bold();
					header.Cell().AlignRight().Text("Amount").Bold();
					header.Cell().ColumnSpan(4).PaddingTop(3).BorderBottom(1).BorderColor(Colors.Black);
				});

				table.Cell().Text($"{_invoice.TotalNights} night(s) @ {_invoice.PricePerNight:N2}");
				table.Cell().AlignRight().Text("Stay");
				table.Cell().AlignRight().Text(_invoice.PricePerNight.ToString("N2"));
				table.Cell().AlignRight().Text(_invoice.SubtotalAmount.ToString("N2"));

				foreach (var fee in _invoice.LineItems)
				{
					table.Cell().Text(fee.FeeName + (fee.IsVATLine ? $" ({fee.FeeValue}%)" : ""));
					table.Cell().AlignRight().Text(fee.FeeCategory);
					table.Cell().AlignRight().Text(fee.FeeCategory == "PERCENTAGE" ? $"{fee.FeeValue}%" : fee.FeeValue.ToString("N2"));
					table.Cell().AlignRight().Text(fee.CalculatedAmount.ToString("N2"));
				}

				if (_invoice.DiscountAmount > 0)
				{
					table.Cell().Text("Discount");
					table.Cell().AlignRight().Text("-");
					table.Cell().AlignRight().Text("-");
					table.Cell().AlignRight().Text($"-{_invoice.DiscountAmount:N2}");
				}
			});
		}

		private void ComposeTotals(IContainer container)
		{
			container.AlignRight().Width(240).Column(col =>
			{
				col.Item().Row(r => { r.RelativeItem().Text("Amount (excl. VAT)"); r.ConstantItem(90).AlignRight().Text(_invoice.AmountExclVAT.ToString("N2")); });
				col.Item().Row(r => { r.RelativeItem().Text($"VAT @ {_invoice.VATRate}%"); r.ConstantItem(90).AlignRight().Text(_invoice.VATAmount.ToString("N2")); });

				col.Item().PaddingTop(3).BorderTop(1).BorderColor(Colors.Black).Row(r =>
				{
					r.RelativeItem().Text("Total (incl. VAT)").Bold();
					r.ConstantItem(90).AlignRight().Text(_invoice.TotalAmount.ToString("N2")).Bold();
				});

				col.Item().Row(r => { r.RelativeItem().Text("Amount Paid"); r.ConstantItem(90).AlignRight().Text(_invoice.AmountPaid.ToString("N2")); });

				if (_invoice.RemainingBalance > 0)
				{
					col.Item().Row(r =>
					{
						r.RelativeItem().Text("Balance Due").FontColor(Colors.Red.Medium).Bold();
						r.ConstantItem(90).AlignRight().Text(_invoice.RemainingBalance.ToString("N2")).FontColor(Colors.Red.Medium).Bold();
					});
				}
			});
		}

		private void ComposePaymentsTable(IContainer container)
		{
			container.Column(col =>
			{
				col.Item().PaddingTop(10).Text("Payment History").Bold();

				col.Item().Table(table =>
				{
					table.ColumnsDefinition(columns =>
					{
						columns.RelativeColumn(2);
						columns.RelativeColumn(2);
						columns.RelativeColumn(3);
						columns.RelativeColumn(2);
					});

					table.Header(header =>
					{
						header.Cell().Text("Type").Bold();
						header.Cell().AlignRight().Text("Amount").Bold();
						header.Cell().Text("Paid Via").Bold();
						header.Cell().AlignRight().Text("Date").Bold();
					});

					foreach (var p in _invoice.Payments)
					{
						table.Cell().Text(p.PaymentType);
						table.Cell().AlignRight().Text($"{p.Amount:N2} {p.CurrencyCode}");
						table.Cell().Text(string.IsNullOrEmpty(p.CardBrand) ? "-" : $"{p.CardBrand} •••• {p.CardLast4}");
						table.Cell().AlignRight().Text(p.PaidDate?.ToString("dd MMM yyyy") ?? "-");
					}
				});
			});
		}
	}
}
