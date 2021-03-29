namespace ZakharovKrilov11_707ISB.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Dasync.Collections;

    public class TfIdfService
    {
        public void CalculateTfIdf()
        {
            var documentIndexesArray = File.ReadLines("Htmls/index.txt")
                .Select(str => str.Split(' ').First())
                .ToArray();

            var documentDictionary = documentIndexesArray
                .ToDictionary(
                    index => index,
                    index => string.Join(Environment.NewLine, File.ReadLines($"Htmls/Document{index}.txt")));
            
            var invIndexDict = File.ReadAllLines("Htmls/InvertedIndexes.txt")
                .ToDictionary(key => key.Split(' ').First(),
                    elem => elem.Substring(elem.IndexOf(' ') + 1)
                        .Split(' ')
                        .ToList());
            
            var lemasDict = File.ReadAllLines("Htmls/Lemmas.txt")
                .ToDictionary(key => key.Split(' ').First(),
                    elem => elem.Substring(elem.IndexOf(' ') + 1)
                        .Split(' ')
                        .ToList());

            var wordsByDocumentsDict = GetWordsByDocumentsDict(invIndexDict, lemasDict, documentDictionary);

            var totalDocumentsCount = (double)documentDictionary.Count;
            var wordsIdfDict = wordsByDocumentsDict
                .ToDictionary(x => x.Key, x => Math.Log(totalDocumentsCount / x.Value.Count));

            var documentsTotalWordsDict = wordsByDocumentsDict
                .SelectMany(x => x.Value)
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.Value));

            var tfIdfByDocumentsDict = GetTfIdfByDocumentsDict(
                wordsByDocumentsDict,
                wordsIdfDict,
                documentsTotalWordsDict);

            WriteToFiles(tfIdfByDocumentsDict);
        }

        private async void WriteToFiles(
            ConcurrentDictionary<string, Dictionary<string, Tuple<double, double>>> tfIdfByDocumentsDict)
        {
            await tfIdfByDocumentsDict.ParallelForEachAsync(async kvp =>
            {
                var (key, dictionary) = kvp;
                await using var writer = new StreamWriter($"Htmls/Tf-Idf/Document{key}.txt");

                foreach (var (word, (idf, tfIdf)) in dictionary.OrderBy(x => x.Key))
                {
                    await writer.WriteLineAsync($"{word} {idf} {tfIdf}");
                }
            }, 10);
        }

        private ConcurrentDictionary<string, Dictionary<string, Tuple<double, double>>> GetTfIdfByDocumentsDict(
            ConcurrentDictionary<string, Dictionary<string, int>> wordsByDocumentsDict,
            Dictionary<string, double> wordsIdfDict,
            Dictionary<string, int> documentsTotalWordsDict)
        {
            var tfIdfByDocumentsDict = new ConcurrentDictionary<string, Dictionary<string, Tuple<double, double>>>();

            Parallel.ForEach(wordsByDocumentsDict, new ParallelOptions {MaxDegreeOfParallelism = 10}, kvp =>
            {
                var (word, wordInDocumentCountDict) = kvp;

                foreach (var (docIndex, wordInDocCount) in wordInDocumentCountDict)
                {
                    var idf = wordsIdfDict[word];
                    var tfIdf = idf * ((double)wordInDocCount / documentsTotalWordsDict[docIndex]);

                    tfIdfByDocumentsDict.AddOrUpdate(
                        docIndex,
                        key => new Dictionary<string, Tuple<double, double>>
                        {
                            {
                                word, new Tuple<double, double>(idf, tfIdf)
                            }
                        },
                        (key, dict) =>
                        {
                            dict[word] = new Tuple<double, double>(idf, tfIdf);
                            return dict;
                        });
                }
            });
            
            return tfIdfByDocumentsDict;
        }

        private ConcurrentDictionary<string, Dictionary<string, int>> GetWordsByDocumentsDict(
            Dictionary<string, List<string>> invIndexDict,
            Dictionary<string, List<string>> lemmas,
            Dictionary<string, string> documentDictionary)
        {
            var wordsByDocumentsDict = new ConcurrentDictionary<string, Dictionary<string, int>>();

            Parallel.ForEach(invIndexDict, new ParallelOptions {MaxDegreeOfParallelism = 10}, kvp =>
            {
                var (word, documentIndexes) = kvp;
                foreach (var documentIndex in documentIndexes)
                {
                    var count = CountWordInCurrentDoc(lemmas[word], documentDictionary[documentIndex]);

                    if (count != 0)
                    {
                        wordsByDocumentsDict.AddOrUpdate(
                            word,
                            key => new Dictionary<string, int>
                            {
                                {
                                    documentIndex, count
                                }
                            },
                            (key, dictionary) =>
                            {
                                dictionary[documentIndex] = count;
                                return dictionary;
                            });
                    }
                }
            });

            return wordsByDocumentsDict;
        }

        private int CountWordInCurrentDoc(IEnumerable<string> lemmas, string htmlDoc) =>
            lemmas.Sum(lemma => Regex.Matches(htmlDoc, @"\b" + lemma + @"\b", RegexOptions.IgnoreCase).Count);
    }
}