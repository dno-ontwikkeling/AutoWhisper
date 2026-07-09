using System.Text.RegularExpressions;

namespace AutoWhisper.Services;

/// <summary>
/// Applies user-defined word corrections to transcribed text.
/// Matching is case-insensitive and whole-word (so "lan" won't touch "Atlanta").
/// </summary>
public static class TextCorrectionService
{
    public static string Apply(string text, IReadOnlyList<WordCorrection>? corrections)
    {
        if (string.IsNullOrEmpty(text) || corrections is null || corrections.Count == 0)
            return text;

        foreach (var correction in corrections)
        {
            if (correction is null || string.IsNullOrWhiteSpace(correction.From))
                continue;

            var from = correction.From.Trim();
            var to = correction.To ?? "";

            // Word boundaries via lookarounds so multi-word phrases work too.
            var pattern = $@"(?<!\w){Regex.Escape(from)}(?!\w)";

            // Escape '$' in the replacement so it isn't treated as a regex group reference.
            var replacement = to.Replace("$", "$$");

            text = Regex.Replace(
                text,
                pattern,
                replacement,
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        return text;
    }
}
