namespace Application.DTO.Responses.FileAttachments;

public sealed record FileContentResponse : IAsyncDisposable, IDisposable
{
    public required Stream Content { get; init; }

    public void Dispose()
    {
        Content.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Content.DisposeAsync();
    }
}