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
        private readonly Scraper _scraper;
        private readonly Tokenizer _tokenizer;
        private readonly Lemmatizer _lemmatizer;
        private readonly InvertedIndex _invertedIndex;

        const string url =
            "https://ru.wikipedia.org/wiki/%D0%9A%D0%B0%D1%82%D0%B5%D0%B3%D0%BE%D1%80%D0%B8%D1%8F:%D0%9F%D0%BE%D1%80%D1%82%D0%B0%D0%BB%D1%8B_%D0%BF%D0%BE_%D0%B0%D0%BB%D1%84%D0%B0%D0%B2%D0%B8%D1%82%D1%83";

        public HomeController(Scraper scraper, Tokenizer tokenizer, Lemmatizer lemmatizer, InvertedIndex invertedIndex)
        {
            _scraper = scraper;
            _tokenizer = tokenizer;
            _lemmatizer = lemmatizer;
            _invertedIndex = invertedIndex;
        }

        public async Task<IActionResult> Index()
        {
            /*var response = await _scraper.CallUrl(url);
            var linkList = _scraper.GetLinks(response)
                .Take(100)
                .ToList();
            
            _scraper.WriteLinksToTxt(linkList);
            
            var htmlDocs = new ConcurrentBag<HtmlDocument>();
            await linkList.ParallelForEachAsync(async link =>
            {
                htmlDocs.Add(_scraper.ParseHtml(await _scraper.CallUrl(link)));
            }, 10);
            
            _scraper.WriteHtmlsToTxt(htmlDocs.ToList());

            var tokens = await _tokenizer.TokenizeDocumentsInFolder();
            _tokenizer.WriteTokensToTxt(tokens);
            */

            //_lemmatizer.Lemmatize("Htmls/Tokens.txt");

            _invertedIndex.MakeInvertedIndexes();
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