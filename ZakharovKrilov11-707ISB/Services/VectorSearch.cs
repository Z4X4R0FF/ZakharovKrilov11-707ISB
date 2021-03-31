using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace ZakharovKrilov11_707ISB.Services
{
    public class VectorSearch
    {
        private bool _isLoaded = false;

        private Dictionary<string, List<int>> _invertedIndexesDict;

        private string[] _documentIndexesArray;

        private List<Dictionary<string, double>> _documentVectors;

        public Dictionary<int, double> PerformVectorSearch(string searchString)
        {
            if (!_isLoaded)
            {
                _invertedIndexesDict = File.ReadAllLines("Htmls/InvertedIndexes.txt")
                    .ToDictionary(key => key.Split(' ').First(),
                        elem => elem.Substring(elem.IndexOf(' ') + 1)
                            .Split(' ')
                            .Select(r => Convert.ToInt32(r))
                            .ToList());
                _documentIndexesArray = File.ReadLines("Htmls/index.txt")
                    .Select(str => str.Split(' ').First())
                    .ToArray();
                _documentVectors = _documentIndexesArray.Select(index =>
                    File.ReadLines($"Htmls/Tf-Idf/Document{index}.txt").ToArray().ToDictionary(
                        key => key.Split(' ').First(),
                        value => Math.Round(Convert.ToDouble(value.Split(' ').Last()), 5))).ToList();
                _isLoaded = true;
            }

            var inputWords = searchString.Split(' ').ToList();
            var wordDictionary = new Dictionary<string, double>();
            foreach (var inputWord in inputWords)
            {
                var equalWordCount = inputWords.Count(word => word == inputWord);
                wordDictionary.Add(inputWord,
                    CalculateWordVector(inputWord, equalWordCount, inputWords.Count));
            }

            wordDictionary = wordDictionary.Where(item => item.Value != 0)
                .ToDictionary(item => item.Key, item => item.Value);
            var answers = new Dictionary<int, double>();

            for (var index = 0; index < _documentVectors.Count; index++)
            {
                var documentVector = _documentVectors[index];
                var helpfulFeatures = documentVector.Where(item => wordDictionary.ContainsKey(item.Key))
                    .ToDictionary(item => item.Key, item => item.Value);
                if (helpfulFeatures.Count != 0)
                {
                    answers.Add(index, Math.Round(helpfulFeatures.Sum(item =>
                                                      item.Value * wordDictionary[item.Key]) /
                                                  (Math.Sqrt(wordDictionary
                                                       .Where(item => helpfulFeatures.ContainsKey(item.Key))
                                                       .Sum(item => Math.Pow(item.Value, 2))) *
                                                   Math.Sqrt(helpfulFeatures.Sum(item => Math.Pow(item.Value, 2)))),
                        5));
                }
                else
                {
                    answers.Add(index, 0);
                }
            }


            return answers.OrderByDescending(item => item.Value)
                .ToDictionary(item => item.Key, item => item.Value);
        }

        private double CalculateWordVector(string word, int equalWordCount, int totalWordCount)
        {
            return !_invertedIndexesDict.ContainsKey(word)
                ? 0.0
                : Math.Round(
                    (double) equalWordCount / totalWordCount *
                    Math.Log((double) 100 / _invertedIndexesDict[word].Count), 5);
        }
    }
}