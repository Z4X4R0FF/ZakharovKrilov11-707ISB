namespace ZakharovKrilov11_707ISB.Controllers
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Dasync.Collections;
    using HtmlAgilityPack;
    using Microsoft.AspNetCore.Mvc;
    using ZakharovKrilov11_707ISB.Services;

    public class SearchEngineController : Controller
    {
        private readonly Scraper _scraper;
        private readonly Tokenizer _tokenizer;
        private readonly Lemmatizer _lemmatizer;
        private readonly InvertedIndex _invertedIndex;
        private readonly TfIdfService _tfIdfService;
        private readonly VectorSearch _vectorSearch;

        public SearchEngineController(
            Scraper scraper,
            Tokenizer tokenizer,
            Lemmatizer lemmatizer,
            InvertedIndex invertedIndex,
            TfIdfService tfIdfService, 
            VectorSearch vectorSearch)
        {
            _scraper = scraper;
            _tokenizer = tokenizer;
            _lemmatizer = lemmatizer;
            _invertedIndex = invertedIndex;
            _tfIdfService = tfIdfService;
            _vectorSearch = vectorSearch;
        }

        public IActionResult Main()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
        
        public IActionResult SearchVector()
        {
            return View();
        }

        public IActionResult TfIdfMain()
        {
            return View();
        }

        public IActionResult CalculateTfIdf()
        {
            _tfIdfService.CalculateTfIdf();
            return View();
        }
        
        public async Task<IActionResult> ProcessUrl(string url)
        {
            var response = await _scraper.CallUrl(url);
            var linkList = _scraper.GetLinks(url, response)
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

            _lemmatizer.Lemmatize("Htmls/Tokens.txt");
            _invertedIndex.MakeInvertedIndexes();

            return View("Search");
        }

        public IActionResult SearchResult(string words)
        {
            return View(_invertedIndex.BoolSearch(words));
        }
        
        public IActionResult SearchVectorResult(string words)
        {
            return View(_vectorSearch.PerformVectorSearch(words));
        }
    }
}