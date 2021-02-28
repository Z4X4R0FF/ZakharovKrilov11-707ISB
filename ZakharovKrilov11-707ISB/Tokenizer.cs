using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text.RegularExpressions;

namespace ZakharovKrilov11_707ISB
{
    public static class Tokenizer
    {
        public static List<string> TokenizeDocumentsInFolder(string storagePath, string separatorRegexPattern)
        {
            var tokens = new List<string>();
            var fileStorage = new List<string>();
            try
            {
                var filePaths = Directory.GetFiles(storagePath, "Document*.txt");
                foreach (var filePath in filePaths)
                {
                    fileStorage.AddRange(File.ReadAllLines(filePath));
                }
            }
            catch (Exception e)
            {
                /* todo */
            }

            foreach (var line in fileStorage)
            {
                tokens.AddRange(TokenizeUsingRegex(line, separatorRegexPattern));
            }

            return tokens;
        }

        private static IEnumerable<string> TokenizeUsingRegex(string input, string separatorRegexPattern)
        {
            return Regex.Split(input, separatorRegexPattern).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        }

        public static void WriteTokensToTxt(IEnumerable<string> tokens)
        {
            using (var writer = new StreamWriter("Htmls/Tokens.txt"))
            {
                foreach (var token in tokens)
                {
                    writer.WriteLine(token);
                }
            }
        }
    }
}