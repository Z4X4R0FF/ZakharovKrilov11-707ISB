using System.Collections.Generic;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using System.Linq;

namespace ZakharovKrilov11_707ISB
{
    public static class Scraper
    {
        public static async Task<string> CallUrl(string fullUrl)
        {
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }

        public static IEnumerable<string> GetLinksFromHtml(HtmlDocument htmlDoc)
        {
            var programmerLinks = htmlDoc.DocumentNode.Descendants("li")
                .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection")).ToList();

            return programmerLinks.Where(r => r.FirstChild.Attributes.Count > 0)
                .Select(r => "https://en.wikipedia.org/" + r.FirstChild.Attributes[0].Value).ToList();
        }

        public static HtmlDocument ParseHtml(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }

        public static void WriteLinksToTxt(List<string> links)
        {
            using (var writer = new StreamWriter("Htmls/index.txt"))
            {
                for (var i = 0; i < links.Count; i++)
                {
                    writer.WriteLine(i + " " + links[i]);
                }
            }
        }

        public static void WriteHtmlsToTxt(List<HtmlDocument> htmlDocuments)
        {
            for (var i = 0; i < htmlDocuments.Count; i++)
            {
                File.WriteAllText($"Htmls/Document{i}.txt", htmlDocuments[i].Text);
            }
        }
    }
}