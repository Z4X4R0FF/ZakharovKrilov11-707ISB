using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZakharovKrilov11_707ISB
{
    public class InvertedIndex
    {
        public void MakeInvertedIndexes()
        {
            var words = File.ReadAllLines("Htmls/Lemmas.txt");
            var documentIndexes = File.ReadAllLines("Htmls/index.txt")
                .Select(str => str.Split(' ').First()).ToArray();
            var dictionary = new ConcurrentDictionary<string, ConcurrentBag<string>>();
            var documentDictionary =
                documentIndexes.ToDictionary(index => index,
                    index => File.ReadAllLines($"Htmls/Document{index}.txt"));

            Parallel.ForEach(documentDictionary, new ParallelOptions {MaxDegreeOfParallelism = 10}, document =>
            {
                foreach (var word in words)
                {
                    var str = word.Split(' ');
                    var wordWasFound = false;
                    foreach (var line in document.Value)
                    {
                        for (var i = 1; i < str.Length; i++)
                        {
                            if (line.Contains(str[i]))
                            {
                                dictionary.AddOrUpdate(str[0],
                                    key => new ConcurrentBag<string> {document.Key},
                                    (key, bag) =>
                                    {
                                        if (!bag.Contains(document.Key))
                                        {
                                            bag.Add(document.Key);
                                        }

                                        return bag;
                                    });
                                wordWasFound = true;
                                break;
                            }
                        }

                        if (wordWasFound) break;
                    }
                }
            });

            WriteInvertedIndicesToFile(dictionary.Keys.OrderBy(key => key)
                .ToDictionary(k => k, k => dictionary[k].OrderBy(r => r).ToList()));
        }

        private async void WriteInvertedIndicesToFile(Dictionary<string, List<string>> dictionary)
        {
            await using var writer = new StreamWriter("Htmls/InvertedIndexes.txt");
            foreach (var (key, value) in dictionary)
            {
                await writer.WriteLineAsync($"{key} {value.Aggregate((x, y) => x + ' ' + y)}");
            }
        }
    }
}