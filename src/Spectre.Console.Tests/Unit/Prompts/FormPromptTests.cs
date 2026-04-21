namespace Spectre.Console.Tests.Unit;

public sealed class FormPromptTests
{
    [Fact]
    public void Should_Throw_Exception_For_Empty_Form()
    {
        // Given
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        var form = new FormPrompt();

        // When
        Action action = () => form.Show(console);

        // Then
        action.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe("Cannot show an empty form. Please add fields first.");
    }

    [Fact]
    public void FormField_Should_Validate_Required_Field()
    {
        // Given
        var field = new TextFormField("name", "Name")
            .IsRequired();

        // When
        var result = field.Validate();

        // Then
        result.Successful.ShouldBeFalse();
    }

    [Fact]
    public void FormField_Should_Validate_Custom_Validator()
    {
        // Given
        var field = new TextFormField("age", "Age")
            .IsRequired()
            .Validate(value =>
            {
                if (value == null || value.Length < 2)
                {
                    return ValidationResult.Error("Age must be at least 2 characters");
                }
                return ValidationResult.Success();
            });

        field.Value = "1";

        // When
        var result = field.Validate();

        // Then
        result.Successful.ShouldBeFalse();
        result.Message.ShouldBe("Age must be at least 2 characters");
    }

    [Fact]
    public void SelectionFormField_Should_Store_Choices()
    {
        // Given
        var field = new SelectionFormField("color", "Favorite Color")
            .AddChoice("red", "Red")
            .AddChoice("blue", "Blue")
            .AddChoice("green", "Green");

        // Then
        field.Choices.Count.ShouldBe(3);
        field.Choices[0].Value.ShouldBe("red");
        field.Choices[0].DisplayText.ShouldBe("Red");
    }

    [Fact]
    public void MultiSelectionFormField_Should_Enforce_MinSelections()
    {
        // Given
        var field = new MultiSelectionFormField("hobbies", "Hobbies")
            .AddChoice("reading")
            .AddChoice("gaming")
            .AddChoice("sports")
            .MinSelections(2);

        field.Value = new List<string> { "reading" };

        // When
        var result = field.Validate();

        // Then
        result.Successful.ShouldBeFalse();
        result.Message.ShouldBe("Please select at least 2 options");
    }

    [Fact]
    public void MultiSelectionFormField_Should_Enforce_MaxSelections()
    {
        // Given
        var field = new MultiSelectionFormField("hobbies", "Hobbies")
            .AddChoice("reading")
            .AddChoice("gaming")
            .AddChoice("sports")
            .MaxSelections(2);

        field.Value = new List<string> { "reading", "gaming", "sports" };

        // When
        var result = field.Validate();

        // Then
        result.Successful.ShouldBeFalse();
        result.Message.ShouldBe("Please select at most 2 options");
    }

    [Fact]
    public void PasswordFormField_Should_Have_Default_Mask()
    {
        // Given
        var field = new PasswordFormField("password", "Password");

        // Then
        field.Mask.ShouldBe('*');
        field.IsRequired.ShouldBeTrue();
    }

    [Fact]
    public void TextFormField_Should_Allow_Empty()
    {
        // Given
        var field = new TextFormField("notes", "Notes")
            .AllowEmpty();

        field.Value = "";

        // When
        var result = field.Validate();

        // Then
        result.Successful.ShouldBeTrue();
    }

    [Fact]
    public void FormResult_Should_Get_Value_Generic()
    {
        // Given
        var result = new FormResult();
        result.AddValue("name", "John");
        result.AddValue("age", 30);

        // When
        var name = result.GetValue<string>("name");
        var age = result.GetValue<int>("age");

        // Then
        name.ShouldBe("John");
        age.ShouldBe(30);
    }

    [Fact]
    public void FormResult_Should_TryGetValue_Generic()
    {
        // Given
        var result = new FormResult();
        result.AddValue("name", "John");

        // When
        var hasName = result.TryGetValue<string>("name", out var name);
        var hasMissing = result.TryGetValue<int>("missing", out var missing);

        // Then
        hasName.ShouldBeTrue();
        name.ShouldBe("John");
        hasMissing.ShouldBeFalse();
        missing.ShouldBe(0);
    }

    [Fact]
    public void FormResult_Should_Get_Value_Indexer()
    {
        // Given
        var result = new FormResult();
        result.AddValue("name", "John");

        // When
        var name = result["name"];

        // Then
        name.ShouldBe("John");
    }

    [Fact]
    public void FormResult_Should_Throw_For_Missing_Field()
    {
        // Given
        var result = new FormResult();

        // When
        Action action = () => result.GetValue<string>("missing");

        // Then
        action.ShouldThrow<KeyNotFoundException>()
            .Message.ShouldBe("Field 'missing' not found in form result");
    }

    [Fact]
    public void FormPrompt_Should_Add_Fields_Via_AddFields()
    {
        // Given
        var form = new FormPrompt();
        var fields = new List<IFormField>
        {
            new TextFormField("name", "Name"),
            new TextFormField("email", "Email")
        };

        // When
        form.AddFields(fields);

        // Then
        form.Fields.Count.ShouldBe(2);
        form.Fields[0].Id.ShouldBe("name");
        form.Fields[1].Id.ShouldBe("email");
    }

    [Fact]
    public void SelectionFormField_DefaultIndex_Should_Set_Default()
    {
        // Given
        var field = new SelectionFormField("color", "Color")
            .AddChoice("red")
            .AddChoice("blue")
            .AddChoice("green")
            .DefaultIndex(1);

        // Then
        field.DefaultIndex.ShouldBe(1);
    }

    [Fact]
    public void MultiSelectionFormField_DefaultIndexes_Should_Set_Defaults()
    {
        // Given
        var field = new MultiSelectionFormField("hobbies", "Hobbies")
            .AddChoice("reading")
            .AddChoice("gaming")
            .AddChoice("sports")
            .DefaultIndexes(0, 2);

        // Then
        field.DefaultIndexes.ShouldNotBeNull();
        field.DefaultIndexes.ShouldBe(new[] { 0, 2 });
    }

    [Fact]
    public void FormField_Should_Apply_ValidationErrorMessage()
    {
        // Given
        var field = new TextFormField("name", "Name")
            .IsRequired()
            .ValidationErrorMessage("Name is required!");

        // When
        var result = field.Validate();

        // Then
        result.Message.ShouldBe("Name is required!");
    }

    [Fact]
    public void FormPrompt_Should_Set_Title()
    {
        // Given
        var form = new FormPrompt()
            .Title("My Form")
            .TitleStyle(Color.Red);

        // Then
        form.Title.ShouldBe("My Form");
        form.TitleStyle.ShouldBe(Color.Red);
    }

    [Fact]
    public void FormPrompt_Should_Set_Focus_And_Error_Style()
    {
        // Given
        var form = new FormPrompt()
            .FocusStyle(Color.Green)
            .ErrorStyle(Color.Yellow);

        // Then
        form.FocusStyle.ShouldBe(Color.Green);
        form.ErrorStyle.ShouldBe(Color.Yellow);
    }

    [Fact]
    public void FormPrompt_Should_Set_InstructionsText()
    {
        // Given
        var form = new FormPrompt()
            .InstructionsText("Custom instructions");

        // Then
        form.InstructionsText.ShouldBe("Custom instructions");
    }
}
