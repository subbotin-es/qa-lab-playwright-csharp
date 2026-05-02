// Models/FormData.cs
namespace QALab.Tests.Models;

// C# records — immutable, value-equality, concise syntax
public record RegistrationFormData(
    string FullName,
    string Email,
    int Age,
    string Phone
);

public record TableRow(
    string Id,
    string Name,
    string Email,
    string Status
);

// Enum for async button states
public enum AsyncButtonState
{
    Default,
    Loading,
    Success,
    Error
}
