using Application.DTO.Enums;
using Application.DTO.Responses.General;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Application.CQRS.PipelineBehaviors;

public class ExceptionHandlerPipelineBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, BaseResponse<TResponse>> where TMessage : IMessage
{
    private readonly ILogger<ExceptionHandlerPipelineBehavior<TMessage, TResponse>> _logger;

    public ExceptionHandlerPipelineBehavior(ILogger<ExceptionHandlerPipelineBehavior<TMessage, TResponse>> logger)
    {
        _logger = logger;
    }

    public async ValueTask<BaseResponse<TResponse>> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, BaseResponse<TResponse>> next)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unknown exception, while execute {Message}", nameof(TMessage));
            return CustomStatusCodes.Unknown;
        }
    }
}

public class ExceptionHandlerPipelineBehavior<TMessage> : IPipelineBehavior<TMessage, BaseResponse> where TMessage : IMessage
{
    private readonly ILogger<ExceptionHandlerPipelineBehavior<TMessage>> _logger;

    public ExceptionHandlerPipelineBehavior(ILogger<ExceptionHandlerPipelineBehavior<TMessage>> logger)
    {
        _logger = logger;
    }

    public async ValueTask<BaseResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, BaseResponse> next)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unknown exception, while execute {Message}", nameof(TMessage));
            return CustomStatusCodes.Unknown;
        }
    }
}