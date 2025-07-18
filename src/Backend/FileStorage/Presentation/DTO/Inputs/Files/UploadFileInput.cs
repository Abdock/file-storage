﻿namespace Presentation.DTO.Inputs.Files;

public record UploadFileInput
{
    public DateTimeOffset? ExpiresAt { get; init; }
    public required IFormFile File { get; init; }
}