namespace Application.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class HttpStatusCodeAttribute : Attribute
{
    public int Status { get; }

    public HttpStatusCodeAttribute(int status)
    {
        Status = status;
    }
}