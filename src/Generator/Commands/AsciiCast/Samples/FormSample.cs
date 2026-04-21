using System.Collections.Generic;
using Spectre.Console;

namespace Generator.Commands.Samples;

internal class FormSample : BaseSample
{
    public override void Run(IAnsiConsole console)
    {
        console.WriteLine();
        console.Write(new Rule("[yellow]Interactive Form[/]").RuleStyle("grey").LeftJustified());
        console.WriteLine();

        var form = new FormPrompt()
            .Title("[bold]User Registration[/]")
            .TitleStyle(Color.Yellow)
            .FocusStyle(Color.Cyan1)
            .ErrorStyle(Color.Red)
            .AddField(new TextFormField("username", "Username")
                .IsRequired()
                .Placeholder("Enter your username")
                .ValidationErrorMessage("[red]Username is required[/]")
                .Validate(username =>
                {
                    if (username == null || username.Length < 3)
                    {
                        return ValidationResult.Error("[red]Username must be at least 3 characters[/]");
                    }
                    return ValidationResult.Success();
                }))
            .AddField(new PasswordFormField("password", "Password")
                .Mask('*')
                .MinLength(6)
                .ValidationErrorMessage("[red]Password must be at least 6 characters[/]"))
            .AddField(new SelectionFormField("role", "Role")
                .AddChoice("admin", "Administrator")
                .AddChoice("moderator", "Moderator")
                .AddChoice("user", "Regular User")
                .DefaultIndex(2)
                .IsRequired())
            .AddField(new MultiSelectionFormField("interests", "Interests")
                .AddChoice("tech", "Technology")
                .AddChoice("gaming", "Gaming")
                .AddChoice("sports", "Sports")
                .AddChoice("music", "Music")
                .AddChoice("reading", "Reading")
                .MinSelections(1)
                .MaxSelections(3)
                .ValidationErrorMessage("[red]Please select 1-3 interests[/]"))
            .AddField(new TextFormField("email", "Email")
                .Placeholder("Optional email address")
                .AllowEmpty()
                .Validate(email =>
                {
                    if (string.IsNullOrEmpty(email))
                    {
                        return ValidationResult.Success();
                    }
                    if (email != null && email.Contains("@"))
                    {
                        return ValidationResult.Success();
                    }
                    return ValidationResult.Error("[red]Invalid email format[/]");
                }));

        console.Write(new Rule("[yellow]Instructions[/]").RuleStyle("grey").LeftJustified());
        console.WriteLine();
        console.WriteLine("[grey]Use Tab/Shift+Tab to move between fields[/]");
        console.WriteLine("[grey]Use Up/Down arrows to select options[/]");
        console.WriteLine("[grey]Use Spacebar to toggle multi-select options[/]");
        console.WriteLine("[grey]Press Enter to submit[/]");
        console.WriteLine();

        var result = form.Show(console);

        console.WriteLine();
        console.Write(new Rule("[green]Form Submitted Successfully[/]").RuleStyle("green").LeftJustified());
        console.WriteLine();

        var table = new Table()
            .AddColumn("[grey]Field[/]")
            .AddColumn("[grey]Value[/]")
            .RoundedBorder()
            .BorderColor(Color.Grey);

        table.AddRow("Username", result.GetValue<string>("username") ?? "N/A");
        table.AddRow("Password", "[grey](masked)[/]");
        table.AddRow("Role", result.GetValue<string>("role") ?? "N/A");

        var interests = result.GetValue<IReadOnlyList<string>>("interests");
        table.AddRow("Interests", interests != null ? string.Join(", ", interests) : "N/A");

        var email = result.GetValue<string>("email");
        table.AddRow("Email", string.IsNullOrEmpty(email) ? "[grey](not provided)[/]" : email);

        console.Write(table);
    }
}
