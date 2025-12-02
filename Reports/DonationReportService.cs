//// Services/Reports/DonationReportService.cs
//using FloodRelief.Api.Models;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;

//namespace FloodRelief.Api.Services.Reports
//{
//    public class DonationReportService
//    {
//        public byte[] GenerateDonationReport(
//            IEnumerable<Donation> donations,
//            DateTime? from,
//            DateTime? to,
//            string? titleSuffix = null)
//        {
//            var docTitle = "Donation Report";
//            if (!string.IsNullOrWhiteSpace(titleSuffix))
//                docTitle += $" - {titleSuffix}";

//            var donationsList = donations.ToList();

//            var document = Document.Create(container =>
//            {
//                container.Page(page =>
//                {
//                    page.Margin(30);
//                    page.Size(PageSizes.A4);
//                    page.PageColor(Colors.White);
//                    page.DefaultTextStyle(x => x.FontSize(10));

//                    page.Header()
//                        .Row(row =>
//                        {
//                            row.RelativeItem()
//                                .Column(col =>
//                                {
//                                    col.Item().Text(docTitle).FontSize(16).SemiBold();
//                                    col.Item().Text(text =>
//                                    {
//                                        text.Span("Generated: ");
//                                        text.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'"));
//                                    }).FontSize(9);
//                                    if (from != null || to != null)
//                                    {
//                                        col.Item().Text(text =>
//                                        {
//                                            text.Span("Period: ");
//                                            text.Span($"{from:yyyy-MM-dd} to {to:yyyy-MM-dd}");
//                                        }).FontSize(9);
//                                    }
//                                });
//                        });

//                    page.Content()
//                        .Table(table =>
//                        {
//                            // columns
//                            table.ColumnsDefinition(columns =>
//                            {
//                                columns.ConstantColumn(70);  // Date
//                                columns.RelativeColumn(2);   // Donor
//                                columns.RelativeColumn(2);   // Item
//                                columns.ConstantColumn(40);  // Qty
//                                columns.ConstantColumn(60);  // Weight
//                                columns.RelativeColumn(2);   // Center
//                            });

//                            // header row
//                            table.Header(header =>
//                            {
//                                header.Cell().Element(CellHeader).Text("Date");
//                                header.Cell().Element(CellHeader).Text("Donor");
//                                header.Cell().Element(CellHeader).Text("Item");
//                                header.Cell().Element(CellHeader).Text("Qty");
//                                header.Cell().Element(CellHeader).Text("Weight (kg)");
//                                header.Cell().Element(CellHeader).Text("Center");
//                            });

//                            foreach (var d in donationsList)
//                            {
//                                table.Cell().Element(CellBody).Text(d.CollectedAt.ToString("yyyy-MM-dd"));
//                                table.Cell().Element(CellBody).Text(d.DonorName);
//                                table.Cell().Element(CellBody).Text(d.ItemDescription);
//                                table.Cell().Element(CellBody).Text(d.Quantity?.ToString() ?? "-");
//                                table.Cell().Element(CellBody).Text(d.WeightKg?.ToString("0.##") ?? "-");
//                                table.Cell().Element(CellBody).Text(d.CollectionPoint?.Name ?? "");
//                            }

//                            static IContainer CellHeader(IContainer container) =>
//                                container
//                                    .PaddingVertical(4)
//                                    .PaddingHorizontal(2)
//                                    .Background(Colors.Grey.Lighten3)
//                                    .Border(0.5f)
//                                    .BorderColor(Colors.Grey.Lighten1)
//                                    .DefaultTextStyle(x => x.SemiBold());

//                            static IContainer CellBody(IContainer container) =>
//                                container
//                                    .PaddingVertical(3)
//                                    .PaddingHorizontal(2)
//                                    .BorderBottom(0.25f)
//                                    .BorderColor(Colors.Grey.Lighten3);
//                        });

//                    page.Footer()
//                        .AlignRight()
//                        .Text(x =>
//                        {
//                            x.Span("Total donations: ");
//                            x.Span(donationsList.Count.ToString()).SemiBold();
//                        });
//                });
//            });

//            return document.GeneratePdf();
//        }
//    }
//}
