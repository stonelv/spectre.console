namespace Spectre.Console;

internal static class LinksDetector
{
    private static readonly string[] _linksSupportedTermPrograms = new[]
    {
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
        "Hyper",
        "ConEmu",
    };

    private static readonly string[] _linksSupportedTerms = new[]
    {
        "xterm-256color",
        "screen-256color",
        "tmux-256color",
        "rxvt-unicode-256color",
    };

    private static readonly string[] _ciEnvironments = new[]
    {
        "GITHUB_ACTIONS",
        "GITLAB_CI",
        "APPVEYOR",
        "TRAVIS",
        "CIRCLECI",
        "CODEBUILD_BUILD_ID",
        "BUILDKITE",
        "TF_BUILD",
    };

    public static bool Detect(
        bool supportsAnsi,
        bool isLegacyConsole,
        IDictionary<string, string>? environmentVariables = null)
    {
        if (!supportsAnsi)
        {
            return false;
        }

        var variables = environmentVariables ?? GetEnvironmentVariables();

        if (HasLinksSupportedTermProgram(variables))
        {
            return true;
        }

        if (HasLinksSupportedTerm(variables))
        {
            return true;
        }

        if (IsCIEnvironment(variables))
        {
            return true;
        }

        if (variables.TryGetValue("WT_SESSION", out var wtSession) && !string.IsNullOrWhiteSpace(wtSession))
        {
            return true;
        }

        if (supportsAnsi && !isLegacyConsole)
        {
            return true;
        }

        return false;
    }

    private static bool HasLinksSupportedTermProgram(IDictionary<string, string> variables)
    {
        if (variables.TryGetValue("TERM_PROGRAM", out var termProgram) && !string.IsNullOrWhiteSpace(termProgram))
        {
            foreach (var program in _linksSupportedTermPrograms)
            {
                if (termProgram.Equals(program, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool HasLinksSupportedTerm(IDictionary<string, string> variables)
    {
        if (variables.TryGetValue("TERM", out var term) && !string.IsNullOrWhiteSpace(term))
        {
            foreach (var supportedTerm in _linksSupportedTerms)
            {
                if (term.Equals(supportedTerm, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsCIEnvironment(IDictionary<string, string> variables)
    {
        foreach (var ciEnv in _ciEnvironments)
        {
            if (variables.ContainsKey(ciEnv))
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
