using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UltimateGuitarDownloader.ConsoleApp
{
    public class Downloader
    {
        public async Task Download(string url, HttpClient client = null)
        {
            var id = url.Split("-").Last();

            if (!int.TryParse(id, out _))
            {
                id = url.Split("/").Last();
            }

            bool isClientNull = client == null;
            if (client == null) client = new HttpClient();
            client = new HttpClient();
            client.DefaultRequestHeaders.Referrer = new Uri(url);

            var response = await client.GetAsync($"https://tabs.ultimate-guitar.com/tab/download?id={id}&session_id=");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("404: " + url);
                return;
            }

            var fileName = response.Content.Headers.FirstOrDefault(h => h.Key == "Content-Disposition").Value.First().Split(";")[1].Split("=")[1].Replace("\"", "");

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            var stream = await response.Content.ReadAsStreamAsync();


            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files"));
            using (var fileStream = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", fileName)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }

            stream.Dispose();

            if (!isClientNull)
                client.Dispose();
        }
    }
}
