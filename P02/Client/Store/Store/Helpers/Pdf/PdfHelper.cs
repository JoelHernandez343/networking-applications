using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Store.Helpers.Pdf
{
    class PdfHelper
    {
        public static async Task<bool> CreateAndSaveFile(string email, int cardNumber)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Pdf Files (*.pdf)|*.pdf|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() != true)
                return false;

            var renderer = await Task.Run(() => CreateFile(email, cardNumber));
            var fileName = dialog.FileName;

            renderer.PdfDocument.Save(fileName);

            Process.Start(fileName);

            return true;
        }

        public static PdfDocumentRenderer CreateFile(string email, int cardNumber)
        {
            var document = new Document();
            var section = document.AddSection();

            var title = section.AddParagraph("Orden de compra");
            title.Format.Font.Size = 20;
            title.Format.SpaceAfter = 24;

            var paragraph = section.AddParagraph($"Correo: {email}");
            paragraph.Format.Font.Bold = true;

            paragraph = section.AddParagraph($"Número de tarjeta: {cardNumber}");
            paragraph.Format.Font.Bold = true;

            // Table column definition
            var table = section.AddTable();
            table.Borders.Width = 0.75;

            // Index
            table.AddColumn(Unit.FromCentimeter(1)).Format.Alignment = ParagraphAlignment.Center;
            table.AddColumn(Unit.FromCentimeter(3)); // Image
            table.AddColumn(Unit.FromCentimeter(4)); // Name
            table.AddColumn(Unit.FromCentimeter(4)); // Description
            table.AddColumn(Unit.FromCentimeter(2)); // Price
            table.AddColumn(Unit.FromCentimeter(2)); // Quantity purchased
            table.AddColumn(Unit.FromCentimeter(2)); // Total

            // Headers
            var row = table.AddRow();
            row.Shading.Color = MigraDoc.DocumentObjectModel.Colors.PaleTurquoise;

            var cell = row.Cells[0];
            cell.AddParagraph("Índice");
            cell = row.Cells[1];
            cell.AddParagraph("Imagen");
            cell = row.Cells[2];
            cell.AddParagraph("Nombre");
            cell = row.Cells[3];
            cell.AddParagraph("Descripción");
            cell = row.Cells[4];
            cell.AddParagraph("Precio");
            cell = row.Cells[5];
            cell.AddParagraph("Cantidad");
            cell = row.Cells[6];
            cell.AddParagraph("Total");


            double total = 0.0;
            int i = 1;
            foreach (var item in ItemList.List)
            {
                if (item.Reserved > 0)
                {
                    double subtotal = item.Reserved * (item.HasDiscount ? item.DiscountPrice : item.NormalPrice);
                    total += subtotal;

                    row = table.AddRow();
                    cell = row.Cells[0];
                    cell.AddParagraph($"{i++}");
                    cell = row.Cells[1];
                    cell.AddImage(MigraDocFilenameFromByteArray(GetBytesFromBitmapImage(item.Images[0]))).Width = Unit.FromCentimeter(2.8);
                    cell = row.Cells[2];
                    cell.AddParagraph($"{item.Name}");
                    cell = row.Cells[3];
                    cell.AddParagraph($"{item.Description}");
                    
                    cell = row.Cells[4];
                    paragraph = cell.AddParagraph($"{item.NormalPrice:C2}");
                    if (item.HasDiscount)
                    {
                        paragraph.Format.Font.Italic = true;
                        paragraph.Format.Font.Size = 10;

                        paragraph = cell.AddParagraph($"{item.DiscountPrice:C2}");
                        paragraph.Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Green;
                    }
                    paragraph.Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Green;

                    cell = row.Cells[5];
                    cell.AddParagraph($"{item.Reserved}");
                    cell = row.Cells[6];
                    cell.AddParagraph($"{subtotal:C2}");
                }
            }

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph($"Total: {total:C2}");
            cell.MergeRight = 6;
            cell.Format.Alignment = ParagraphAlignment.Right;

            var renderer = new PdfDocumentRenderer { Document = document };
            renderer.RenderDocument();

            return renderer;
        }

        static byte[] GetBytesFromBitmapImage(BitmapImage image)
        {
            var data = new byte[0];
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        static string MigraDocFilenameFromByteArray(byte[] image) => "base64:" + Convert.ToBase64String(image);
    }
}
