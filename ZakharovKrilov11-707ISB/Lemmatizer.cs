using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeepMorphy;

namespace ZakharovKrilov11_707ISB
{
    public class Lemmatizer
    {
        public void Lemmatize(string filepath)
        {
            var dictionary = new ConcurrentDictionary<string, ConcurrentBag<string>>();
            var words = File.ReadLines(filepath);
            var morph = new MorphAnalyzer(withLemmatization: true);
            var results = morph.Parse(words).ToArray();

            Parallel.ForEach(results, new ParallelOptions {MaxDegreeOfParallelism = 10}, morphInfo =>
            {
                var lemma = morphInfo.BestTag.Lemma;
                var originalWord = morphInfo.Text;
                dictionary.AddOrUpdate(
                    lemma,
                    key => new ConcurrentBag<string> {originalWord},
                    (key, bag) =>
                    {
                        if (!bag.Contains(originalWord))
                        {
                            bag.Add(originalWord);
                        }

                        return bag;
                    });
            });


            WriteLemmasToFile(
                dictionary.Keys.OrderBy(key => key)
                    .ToDictionary(k => k, k => dictionary[k].ToList()));
        }

        private async void WriteLemmasToFile(Dictionary<string, List<string>> dictionary)
        {
            await using var writer = new StreamWriter("Htmls/Lemmas.txt");
            foreach (var (key, value) in dictionary)
            {
                await writer.WriteLineAsync($"{key} {value.Aggregate((x, y) => x + ' ' + y)}");
            }
        }
    }
}