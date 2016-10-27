using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Humanizer;

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
	        var inputString = @"
Платье 12500-00.  2 Италия
Платье 11500-00.  2 Италия
Туника. 14500-00  2 Италия
Джемпер  9500-00.  10 Италия
Джемпер. 10500-00. 10 Италия
Шапка.    3950-00.  6 Италия
Туника.  10500-00.  4 Италия
джемпер.  12500-00.   6 Италия
жакет.         13500-00.   4 Италия
джемпер.   11500-00.    6 Италия
шапка.         4250-00.    6 Италия
";

	        var inputStrings =
		        inputString.Replace(".", "")
			        .Replace("-00", "")
			        .Split(new[] {Environment.NewLine}, StringSplitOptions.None)
			        .Where(x => x.IsNotEmpty())
			        .Select(x => x.Trim().Transform(To.TitleCase));

	        var nfi = (NumberFormatInfo) CultureInfo.InvariantCulture.NumberFormat.Clone();
	        nfi.NumberGroupSeparator = " ";

			var input = inputStrings
		        .Select(x => Regex.Match(x, @"(?<name>[А-Яа-я]+)\s+(?<price>\d+)\s+(?<count>\d+)\s+(?<country>[А-Яа-я]+)"))
		        .Select(m => new Item
		        {
			        Name = m.Groups["name"].Value,
					Price = m.Groups["price"].Value.ToDecimal().ToString("#,0", nfi),
					Country = m.Groups["country"].Value,
					Count = m.Groups["count"].Value.ToInt()
		        });

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

	public static class StringExtensions
	{
		public static int ToInt(this string s)
		{
			return int.Parse(s);
		}

		public static decimal ToDecimal(this string s)
		{
			return decimal.Parse(s);
		}

		public static bool IsNotEmpty(this string s)
		{
			return s.Trim() != "";
		}
	}
}