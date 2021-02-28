using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZakharovKrilov11_707ISB.Models;

namespace ZakharovKrilov11_707ISB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            /*const string url = "https://en.wikipedia.org/wiki/List_of_programmers";
            var response = Scraper.CallUrl(url).Result;
            var linkList = Scraper.GetLinksFromHtml(Scraper.ParseHtml(response)).Take(100).ToList();
            Scraper.WriteLinksToTxt(linkList);
            var htmlDocList = linkList.Select(link => Scraper.CallUrl(link).Result)
                .Select(Scraper.ParseHtml).ToList();
            Scraper.WriteHtmlsToTxt(htmlDocList);
            */
            var tokens = Tokenizer.TokenizeDocumentsInFolder("Htmls", " ");
            Tokenizer.WriteTokensToTxt(tokens);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}