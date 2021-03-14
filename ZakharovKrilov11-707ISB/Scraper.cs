﻿using System.Collections.Generic;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Linq;

namespace ZakharovKrilov11_707ISB
{
    public class Scraper
    {
        public Task<string> CallUrl(string fullUrl)
        {
            var httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            httpClient.DefaultRequestHeaders.Accept.Clear();
            
            return httpClient.GetStringAsync(fullUrl);
        }

        public IEnumerable<string> GetLinks(string html)
        {
            var htmlDoc = ParseHtml(html);
            var programmerLinks = htmlDoc.DocumentNode.Descendants("li")
                .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection"))
                .ToList();

            return programmerLinks
                .Where(r => r.FirstChild.Attributes.Count > 0)
                .Select(r => "https://ru.wikipedia.org/" + r.FirstChild.Attributes[0].Value)
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
                writer.WriteLine(i + " " + links[i]);
            }
        }

        public void WriteHtmlsToTxt(List<HtmlDocument> htmlDocuments)
        {
            for (var i = 0; i < htmlDocuments.Count; i++)
            {
                File.WriteAllText($"Htmls/Document{i}.txt", htmlDocuments[i].ParsedText);
            }
        }
    }
}