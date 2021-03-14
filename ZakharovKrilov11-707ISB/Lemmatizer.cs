using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeepMorphy;

namespace ZakharovKrilov11_707ISB
{
    public class Lemmatizer
    {
        public static void Lemmatize(string filepath)
        {
            var dictionary = new Dictionary<string, List<string>>();
            var words = File.ReadLines(filepath);
            var morph = new MorphAnalyzer(withLemmatization: true);
            var results = morph.Parse(words).ToArray();
            foreach (var morphInfo in results)
            {
                var lemma = morphInfo.BestTag.Lemma;
                if (!dictionary.ContainsKey(lemma))
                {
                    dictionary.Add(lemma, new List<string> {morphInfo.Text});
                }
                else
                {
                    dictionary[lemma].Add(morphInfo.Text);
                }
            }

            WriteLemmasToFile(dictionary);
        }

        private static void WriteLemmasToFile(Dictionary<string, List<string>> dictionary)
        {
            using (var writer = new StreamWriter("Htmls/Lemmas.txt"))
            {
                foreach (var (key, value) in dictionary)
                {
                    writer.WriteLine($"{key} {value.Aggregate((x, y) => x + ' ' + y)}");
                }
            }
        }
    }
}