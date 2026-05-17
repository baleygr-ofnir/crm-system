using System.Globalization;
using System.Text;

namespace azfunc.Helpers;

public static class StringHelper
{
    // Normalises text including remove accents/diacritics
    public static string TextNormalizer(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var normalized = input.Normalize(NormalizationForm.FormD);
        var outputBuilder = new StringBuilder();

        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                outputBuilder.Append(c);
        }

        var output = outputBuilder.ToString().Normalize(NormalizationForm.FormC);

        return output;
    }
}