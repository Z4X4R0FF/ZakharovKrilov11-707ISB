namespace ZakharovKrilov11_707ISB
{
    using System.Collections.Concurrent;
    using System.Text;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Dasync.Collections;
    using HtmlAgilityPack;
    using StopWord;

    public class Tokenizer
    {
        private readonly char[] _splitters;
        private string _pattern;
        private string[] _rusStopWords;

        public Tokenizer()
        {
            _splitters = new[]
            {
                '\n', '\t', '\r', ':', ';', '(', ')', '.', ',', ' ', '[', ']', '-', '"', '{', '}', '!', '?',
                '@', '$', '=', '^', '/', '\\', '°', '#', '*', '|', '§', '·', '—', '»', '«'
            };
            _pattern = @"<(.|\n)*?>";
            _rusStopWords = StopWords.GetStopWords("ru");

        }

        public async Task<List<string>> TokenizeDocumentsInFolder()
        {
            var tokens = new ConcurrentBag<string>();
            var fileNames = Directory.GetFiles("Htmls", "Document*.txt");
            
            await fileNames.ParallelForEachAsync(async fileName =>
            {
                var tokensList = GetTokens(await File.ReadAllTextAsync(fileName, Encoding.UTF8));
                foreach (var token in tokensList)
                {
                    tokens.Add(token);
                }
            }, 10);

            return tokens.ToList();
        }

        private List<string> GetTokens(string htmlDocument)
        {
            var document = new HtmlDocument();
            document.LoadHtml(htmlDocument);
            var body = document.GetElementbyId("bodyContent").InnerHtml;
            
            var step1 = Regex.Replace(body, "<script.*?script>", " ", RegexOptions.Singleline);
            var step2 = Regex.Replace(step1, "<style.*?style>", " ", RegexOptions.Singleline);
            var step3 = Regex.Replace(step2, "&#.*?;", " ");
            var step4 = Regex.Replace(step3, _pattern, " ");
            var step5 = Regex.Replace(step4, "\t", " ");
            var textOnly = Regex.Replace(step5, "[\r\n]+", "\r\n");
            var step6 = Regex.Replace(textOnly, @"\s+", " ");

            return step6.Split(_splitters)
                .Where(x=> x != string.Empty)
                .Select(x => x.ToLower())
                .Where(x =>  x.All(c => !char.IsDigit(c)))
                .Where(x => x.All(c => c >= 'а' && c <= 'я'))
                .Where(x => !_rusStopWords.Contains(x))
                .ToList();
        }

        public async void WriteTokensToTxt(List<string> tokens)
        {
            await using var writer = new StreamWriter("Htmls/Tokens.txt");
            foreach (var token in tokens)
            {
                await writer.WriteLineAsync(token);
            }
        }
    }
}