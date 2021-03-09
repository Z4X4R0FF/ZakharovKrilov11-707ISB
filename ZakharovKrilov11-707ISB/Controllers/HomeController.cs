using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZakharovKrilov11_707ISB.Models;
using Dasync.Collections;

namespace ZakharovKrilov11_707ISB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // const string url =
            //     "https://ru.wikipedia.org/wiki/%D0%9A%D0%B0%D1%82%D0%B5%D0%B3%D0%BE%D1%80%D0%B8%D1%8F:%D0%9F%D0%BE%D1%80%D1%82%D0%B0%D0%BB%D1%8B_%D0%BF%D0%BE_%D0%B0%D0%BB%D1%84%D0%B0%D0%B2%D0%B8%D1%82%D1%83";
            // var response = await Scraper.CallUrl(url);
            // var linkList = Scraper.GetLinksFromHtml(Scraper.ParseHtml(response)).Take(100).ToList();
            // Scraper.WriteLinksToTxt(linkList);
            // var htmlDocList = new ConcurrentBag<HtmlDocument>();
            //
            // await linkList.ParallelForEachAsync(async link =>
            // {
            //     var result = await Scraper.CallUrl(link);
            //     htmlDocList.Add(Scraper.ParseHtml(result));
            // }, 10);
            // Scraper.WriteHtmlsToTxt(htmlDocList.ToList());

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