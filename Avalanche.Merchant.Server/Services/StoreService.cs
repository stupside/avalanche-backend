using Avalanche.Identity.Client;
using Avalanche.Merchant.Application.Features.Stores;
using Avalanche.Merchant.Store;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Avalanche.Merchant.Server.Services;

[Authorize]
public class StoreService : Merchant.Store.StoreService.StoreServiceBase
{
    private readonly ISender _sender;

    public StoreService(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<CreateStoreRpc.Types.Response> Create(CreateStoreRpc.Types.Command request,
        ServerCallContext context)
    {
        var user = context.GetHttpContext().GetUserId();

        var command = new CreateStore.Command(user, request.Name, request.Description, request.Email);

        var response = await _sender.Send(command);

        return new CreateStoreRpc.Types.Response
        {
            StoreId = response.ToString()
        };
    }

    public override async Task<Empty> Update(UpdateStoreRpc.Types.Command request,
        ServerCallContext context)
    {
        var user = context.GetHttpContext().GetUserId();

        if (Guid.TryParse(request.StoreId, out var store) is false)
            throw new ValidationException("Invalid guid");

        var command = new UpdateStore.Command(store, user, request.Description, request.Email);

        await _sender.Send(command);

        return new Empty();
    }

    [AllowAnonymous]
    public override async Task<GetOneStoreRpc.Types.Response> GetOne(GetOneStoreRpc.Types.Request request,
        ServerCallContext context)
    {
        if (Guid.TryParse(request.StoreId, out var store) is false)
            throw new ValidationException("Invalid guid");

        var command = new GetStore.Request(store);

        var response = await _sender.Send(command);

        return new GetOneStoreRpc.Types.Response
        {
            StoreId = store.ToString(),
            Name = response.Name,
            Description = response.Description,
            Email = response.Email,
            Logo = response.Logo
        };
    }

    [AllowAnonymous]
    public override async Task<GetManyStoresRpc.Types.Response> GetManyByName(
        GetManyStoresRpc.Types.RequestByName request,
        ServerCallContext context)
    {
        var command = new GetStores.RequestByName(request.NameSearch);

        var response = await _sender.Send(command, context.CancellationToken);

        var items = response.Select(m => new GetManyStoresRpc.Types.Response.Types.Item
        {
            StoreId = m.StoreId.ToString(),
            Name = m.Name,
            Description = m.Description,
            Logo = m.Logo
        });

        return new GetManyStoresRpc.Types.Response
        {
            Items = { items }
        };
    }

    public override async Task<GetManyStoresRpc.Types.Response> GetManyByIdentifiers(
        GetManyStoresRpc.Types.RequestByIdentifiers request, ServerCallContext context)
    {
        var command = new GetStores.RequestByIdentifiers(request.Identifiers.Select(Guid.Parse));

        var response = await _sender.Send(command, context.CancellationToken);

        var items = response.Select(m => new GetManyStoresRpc.Types.Response.Types.Item
        {
            StoreId = m.StoreId.ToString(),
            Name = m.Name,
            Description = m.Description,
            Logo = m.Logo
        });

        return new GetManyStoresRpc.Types.Response
        {
            Items = { items }
        };
    }
}