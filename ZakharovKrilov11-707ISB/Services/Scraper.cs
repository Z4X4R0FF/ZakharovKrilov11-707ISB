namespace ZakharovKrilov11_707ISB.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using HtmlAgilityPack;

    public class Scraper
    {
        public Task<string> CallUrl(string fullUrl)
        {
            var httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            httpClient.DefaultRequestHeaders.Accept.Clear();

            return httpClient.GetStringAsync(fullUrl);
        }

        public IEnumerable<string> GetLinks(string url, string html)
        {
            var uri = new Uri(url);
            var mainPage = uri.GetLeftPart(UriPartial.Authority) + '/';

            var htmlDoc = ParseHtml(html);
            var programmerLinks = htmlDoc.DocumentNode.Descendants("li")
                .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection"))
                .ToList();

            return programmerLinks
                .Where(r => r.FirstChild.Attributes.Count > 0)
                .Select(r => mainPage + r.FirstChild.Attributes[0].Value)
                .ToList();
        }

        public HtmlDocument ParseHtml(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }

        public void WriteLinksToTxt(List<string> links)
        {
            using var writer = new StreamWriter("Htmls/index.txt");
            for (var i = 0; i < links.Count; i++)
            {
                if (i != 0 && i < 10) writer.WriteLine($"0{i} {links[i]}");
                else writer.WriteLine(i + " " + links[i]);
            }
        }

        public void WriteHtmlsToTxt(List<HtmlDocument> htmlDocuments)
        {
            for (var i = 0; i < htmlDocuments.Count; i++)
            {
                if (i != 0 && i < 10) File.WriteAllText($"Htmls/Document0{i}.txt", htmlDocuments[i].ParsedText);
                else File.WriteAllText($"Htmls/Document{i}.txt", htmlDocuments[i].ParsedText);
            }
        }
    }
}