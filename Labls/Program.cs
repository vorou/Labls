using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Labls
{
    internal class Program
    {
        private static string GetPath(string name)
        {
            return Path.Combine(@"C:\Users\vorou\Desktop", name);
        }

        private static void Main()
        {
            var input = new[]
            {
                new Item {Name = "Свитер женский", Price = "9500", Country = "Италия", Count = 5},
                new Item {Name = "Кардиган женский", Price = "19500", Country = "Франция", Count = 10},
                new Item {Name = "Кардиган женский", Price = "19500", Country = "Франция", Count = 30}
            };

            File.Delete(GetPath("output-labels.docx"));
            File.Copy(GetPath("labels.docx"), GetPath("output-labels.docx"));
            var wordprocessingDocument = WordprocessingDocument.Open(GetPath("output-labels.docx"), true);
            var body = wordprocessingDocument.MainDocumentPart.Document.Body;

            var table = body.GetFirstChild<Table>();
            var cellsPerPage = table.Descendants<TableCell>().ToArray();
            Console.Out.WriteLine($"{cellsPerPage.Length} labels per page");
            var pages = (int) Math.Ceiling(input.Sum(x => x.Count)/(double) cellsPerPage.Length);
            Console.Out.WriteLine($"{pages} pages total");

            for (var i = 0; i < pages - 1; i++)
                body.AppendChild(table.CloneNode(true));

            var cells = body.Descendants<TableCell>().ToArray();

            var firstCell = cells.First();
            var template = firstCell.CloneNode(true);

            var cell = 0;
            foreach (var item in input)
            {
                var label = template.CloneNode(true);
                foreach (var text in label.Descendants<Text>())
                    text.Text = text.Text.Replace("{Name}", item.Name)
                        .Replace("{Price}", item.Price)
                        .Replace("{Country}", item.Country);
                Console.Out.WriteLine($"count={item.Count}");
                for (var i = 0; i < item.Count; i++)
                {
                    Console.Out.WriteLine($"i={i}");
                    cells[cell].Parent.ReplaceChild(label.CloneNode(true), cells[cell]);
                    cell++;
                }
            }

            wordprocessingDocument.Close();
            Process.Start(GetPath("output-labels.docx"));
        }
    }

    internal class Item
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Country { get; set; }
        public int Count { get; set; }
    }
}