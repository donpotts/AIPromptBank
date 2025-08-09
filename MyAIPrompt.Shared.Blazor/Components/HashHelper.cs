using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MyAIPrompt.Shared.Blazor.Components
{
    public static class HashHelper
    {
        public static string ComputeHash(string title, string content, string? salt = null, int version = 1)
        {
            string normalizedTitle = Normalize(title);
            string normalizedContent = Normalize(content);
            string input = $"{normalizedTitle}|{normalizedContent}";

            if (!string.IsNullOrEmpty(salt))
            {
                input = $"{salt}|{input}";
            }

            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(bytes);

            string hash = Convert.ToHexString(hashBytes);
            return $"v{version}:{hash}";
        }

        private static string Normalize(string input)
        {
            return input?.Trim().ToLowerInvariant() ?? string.Empty;
        }
    

        public static int ComputeLevenshteinDistance(string a, string b)
            {
                if (string.IsNullOrEmpty(a)) return b.Length;
                if (string.IsNullOrEmpty(b)) return a.Length;

                int[,] matrix = new int[a.Length + 1, b.Length + 1];

                for (int i = 0; i <= a.Length; i++) matrix[i, 0] = i;
                for (int j = 0; j <= b.Length; j++) matrix[0, j] = j;

                for (int i = 1; i <= a.Length; i++)
                {
                    for (int j = 1; j <= b.Length; j++)
                    {
                        int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                        matrix[i, j] = Math.Min(
                            Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                            matrix[i - 1, j - 1] + cost
                        );
                    }
                }

                return matrix[a.Length, b.Length];
            }

        }
}