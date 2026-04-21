namespace Spectre.Console;

internal static class UnicodeDetector
{
    private static readonly string[] _unicodeEncodingNames = new[]
    {
        "Unicode",
        "UTF-8",
        "UTF-16",
        "UTF-32",
        "utf-8",
        "utf-16",
        "utf-32",
    };

    private static readonly string[] _utf8LocaleIndicators = new[]
    {
        "UTF-8",
        "UTF8",
        "utf-8",
        "utf8",
    };

    public static bool Detect(
        Encoding encoding,
        IDictionary<string, string>? environmentVariables = null)
    {
        var variables = environmentVariables ?? GetEnvironmentVariables();

        if (IsUnicodeEncoding(encoding))
        {
            return true;
        }

        if (HasUtf8Locale(variables))
        {
            return true;
        }

        if (HasUnicodeTerm(variables))
        {
            return true;
        }

        return false;
    }

    private static bool IsUnicodeEncoding(Encoding encoding)
    {
        var webName = encoding.WebName;
        var encodingName = encoding.EncodingName;

        foreach (var name in _unicodeEncodingNames)
        {
            if (webName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                encodingName.Contains(name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasUtf8Locale(IDictionary<string, string> variables)
    {
        var localeVars = new[] { "LANG", "LC_ALL", "LC_CTYPE" };

        foreach (var varName in localeVars)
        {
            if (variables.TryGetValue(varName, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                foreach (var indicator in _utf8LocaleIndicators)
                {
                    if (value.Contains(indicator, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static bool HasUnicodeTerm(IDictionary<string, string> variables)
    {
        if (variables.TryGetValue("TERM", out var term) && !string.IsNullOrWhiteSpace(term))
        {
            if (term.Contains("256color", StringComparison.OrdinalIgnoreCase) ||
                term.Contains("xterm", StringComparison.OrdinalIgnoreCase) ||
                term.Contains("screen", StringComparison.OrdinalIgnoreCase) ||
                term.Contains("tmux", StringComparison.OrdinalIgnoreCase) ||
                term.Contains("rxvt-unicode", StringComparison.OrdinalIgnoreCase) ||
                term.Contains("alacritty", StringComparison.OrdinalIgnoreCase) ||
                term.Contains("kitty", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static IDictionary<string, string> GetEnvironmentVariables()
    {
        return Environment.GetEnvironmentVariables()
            .Cast<System.Collections.DictionaryEntry>()
            .Aggregate(
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                (dictionary, entry) =>
                {
                    var key = (string)entry.Key;
                    if (!dictionary.TryGetValue(key, out _))
                    {
                        dictionary.Add(key, entry.Value as string ?? string.Empty);
                    }

                    return dictionary;
                },
                dictionary => dictionary);
    }
}
