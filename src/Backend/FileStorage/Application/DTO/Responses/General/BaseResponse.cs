using Application.DTO.Enums;

namespace Application.DTO.Responses.General;

public record BaseResponse<TResponse>
{
    public required CustomStatusCodes StatusCode { get; init; }
    public bool IsSuccess => StatusCode == CustomStatusCodes.Ok;
    public TResponse? Response { get; init; }

    public static implicit operator BaseResponse<TResponse>(TResponse response)
    {
        return new BaseResponse<TResponse>
        {
            StatusCode = CustomStatusCodes.Ok,
            Response = response
        };
    }

    public static implicit operator BaseResponse<TResponse>(CustomStatusCodes statusCode)
    {
        return new BaseResponse<TResponse>
        {
            StatusCode = statusCode
        };
    }
}

public record BaseResponse : BaseResponse<object>
{
    public static implicit operator BaseResponse(CustomStatusCodes statusCode)
    {
        return new BaseResponse
        {
            StatusCode = statusCode
        };
    }
}