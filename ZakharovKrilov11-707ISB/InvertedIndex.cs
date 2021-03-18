using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DeepMorphy;

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
                            if (Regex.IsMatch(line, str[i], RegexOptions.IgnoreCase))
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

        public string BoolSearch(string searchString)
        {
            var str = searchString.Split(' ');
            var wordToSearch = new List<string>();
            var wordToSkip = new List<string>();

            foreach (var word in str)
            {
                switch (word[0])
                {
                    case '!':
                        wordToSkip.Add(word.Substring(1));
                        break;
                    default:
                        wordToSearch.Add(word);
                        break;
                }
            }

            var morph = new MorphAnalyzer(withLemmatization: true);
            var searchWords = morph.Parse(wordToSearch).Select(r => r.BestTag.Lemma).ToList();
            var skipWords = morph.Parse(wordToSkip).Select(r => r.BestTag.Lemma).ToList();

            var invIndexDict = File.ReadAllLines("Htmls/InvertedIndexes.txt")
                .ToDictionary(key => key.Split(' ').First(),
                    elem => elem.Substring(elem.IndexOf(' ') + 1)
                        .Split(' ').Select(r => Convert.ToInt32(r)).ToList());

            var skipIndexes = new List<int>();
            var foundIndexes = new List<int>();

            foreach (var skipWord in skipWords.Where(skipWord => invIndexDict.ContainsKey(skipWord)))
            {
                skipIndexes.AddRange(invIndexDict[skipWord].Where(index => !skipIndexes.Contains(index)));
            }

            foreach (var searchWord in searchWords)
            {
                if (invIndexDict.ContainsKey(searchWord))
                {
                    if (foundIndexes.Count == 0)
                    {
                        foundIndexes.AddRange(invIndexDict[searchWord]);
                    }
                    else
                    {
                        var wordIndexes = invIndexDict[searchWord];
                        foundIndexes = foundIndexes.Where(r => wordIndexes.Contains(r)).ToList();
                    }
                }
                else
                {
                    return $"Word {searchWord} not found";
                }
            }

            if (foundIndexes.Count == 0)
            {
                return "Nothing was found";
            }

            foundIndexes = foundIndexes.Except(skipIndexes).ToList();

            return foundIndexes.Count == 0
                ? "Nothing was found"
                : $"Documents : {string.Join(' ', foundIndexes.Select(x => x.ToString()).ToArray())}";
        }
    }
}