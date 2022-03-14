using HtmlAgilityPack;
using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UltimateGuitarDownloader.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var downloader = new Downloader();
            int counter = 1;

            for (int i = 0; i < 100; i++)
            {
                var url = $"https://www.ultimate-guitar.com/explore?order=hitsdailygroup_desc&type[]=Pro&page={i+1}";
                var content = await (await client.GetAsync(url)).Content.ReadAsStringAsync();

                var doc = new HtmlDocument();
                doc.LoadHtml(content);

                var dataContent = doc.DocumentNode.SelectSingleNode("//div[@class='js-store']").GetAttributeValue("data-content", "");
                var matches = Regex.Matches(dataContent, "https:\\/\\/tabs\\.ultimate-guitar\\.com\\/[^&]*");

                foreach (var match in matches)
                {
                    Console.WriteLine($"{counter} Downloading at: " + match.ToString());
                    try
                    {
                        await downloader.Download(match.ToString(), client);
                        counter++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occured while downloading " + ex.Message);
                        continue;
                    }
                }
            }

            Console.Write("Press any key to close the program");
            Console.ReadKey();
        }
    }
}
