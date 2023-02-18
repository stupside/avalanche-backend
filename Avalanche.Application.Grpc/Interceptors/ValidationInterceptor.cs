using System.Text.Json;
using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Avalanche.Application.Grpc.Interceptors;

public class ValidationInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, JsonSerializer.Serialize(new
            {
                ex.Errors, ex.Message
            })));
        }
    }
}