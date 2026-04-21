namespace Spectre.Console;

internal static class EmojiDetector
{
    private static readonly string[] _emojiSupportedTerms = new[]
    {
        "xterm-256color",
        "screen-256color",
        "tmux-256color",
        "rxvt-unicode-256color",
        "rxvt-256color",
        "iTerm.app",
        "Apple_Terminal",
        "vscode",
        "vscode-insiders",
        "Terminus",
        "WindowsTerminal",
        "alacritty",
        "kitty",
        "wezterm",
        "ghostty",
    };

    private static readonly string[] _ciEmojiSupportedTerms = new[]
    {
        "GITHUB_ACTIONS",
        "GITLAB_CI",
        "APPVEYOR",
        "TRAVIS",
        "CIRCLECI",
        "CODEBUILD_BUILD_ID",
        "BUILDKITE",
    };

    public static bool Detect(bool supportsUnicode, IDictionary<string, string>? environmentVariables = null)
    {
        if (!supportsUnicode)
        {
            return false;
        }

        var variables = environmentVariables ?? GetEnvironmentVariables();

        if (variables.TryGetValue("NO_EMOJI", out var noEmoji) && !string.IsNullOrWhiteSpace(noEmoji))
        {
            return false;
        }

        if (variables.TryGetValue("FORCE_EMOJI", out var forceEmoji) && !string.IsNullOrWhiteSpace(forceEmoji))
        {
            return true;
        }

        if (_ciEmojiSupportedTerms.Any(ci => variables.ContainsKey(ci)))
        {
            return true;
        }

        if (variables.TryGetValue("TERM_PROGRAM", out var termProgram))
        {
            if (!string.IsNullOrWhiteSpace(termProgram))
            {
                if (_emojiSupportedTerms.Any(t => termProgram.Contains(t, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
        }

        if (variables.TryGetValue("TERM", out var term))
        {
            if (!string.IsNullOrWhiteSpace(term))
            {
                if (_emojiSupportedTerms.Any(t => term.Contains(t, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (variables.TryGetValue("WT_SESSION", out var wtSession) && !string.IsNullOrWhiteSpace(wtSession))
            {
                return true;
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return true;
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
