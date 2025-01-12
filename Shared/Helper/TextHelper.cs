using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatGptMiniApp.Shared.Helper
{
    public static class TextHelper
    {
        // A rough check for the presence of at least N Persian characters.
        public static bool IsLikelyPersian(string text, int threshold = 5)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            // Regex covering Persian/Arabic code block: \u0600-\u06FF
            // This also includes Arabic-specific characters, so it’s not purely Persian,
            // but Persian is in that range.
            var pattern = new Regex(@"[\u0600-\u06FF]");
            var matches = pattern.Matches(text);

            // If the count of Persian-range characters >= threshold, we say it's likely Persian.
            return matches.Count >= threshold;
        }
        public static string MarkupToHtml(string text)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var result = Markdown.ToHtml(text.Replace("<br>", "\n"), pipeline);
            return result;
        }
    }
}
