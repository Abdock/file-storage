using Persistence.Enums;

namespace Presentation.DTO.Inputs.ApiKeys;

public class CreateApiKeyInput
{
    public required string Name { get; init; }
    public required Permission[] Permissions { get; init; }
}