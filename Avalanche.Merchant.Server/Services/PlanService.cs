using Avalanche.Identity.Client;
using Avalanche.Merchant.Application.Features.Plans;
using Avalanche.Merchant.Plan;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Avalanche.Merchant.Server.Services;

[Authorize]
public class PlanService : Merchant.Plan.PlanService.PlanServiceBase
{
    private readonly ISender _sender;

    public PlanService(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<Empty> Order(OrderPlanRpc.Types.Command request, ServerCallContext context)
    {
        if (Guid.TryParse(request.PlanId, out var plan) is false)
            throw new ValidationException("Invalid guid");

        var user = context.GetHttpContext().GetUserId();

        var command = new OrderPlan.Command(plan, user,
            DateTimeOffset.UtcNow.AddDays(request.AvailableInDays).AddSeconds(30));

        await _sender.Send(command);

        return new Empty();
    }

    public override async Task<CreatePlanRpc.Types.Response> Create(CreatePlanRpc.Types.Command request,
        ServerCallContext context)
    {
        var user = context.GetHttpContext().GetUserId();

        if (Guid.TryParse(request.StoreId, out var store) is false)
            throw new ValidationException("Invalid guid");

        var command = new CreatePlan.Command(
            store,
            user,
            request.Name,
            request.Validity.ToTimeSpan(),
            request.Price
        );

        var response = await _sender.Send(command);

        return new CreatePlanRpc.Types.Response
        {
            PlanId = response.ToString()
        };
    }

    public override async Task<Empty> Delete(DeletePlanRpc.Types.Command request,
        ServerCallContext context)
    {
        var user = context.GetHttpContext().GetUserId();

        if (Guid.TryParse(request.PlanId, out var plan) is false)
            throw new ValidationException("Invalid guid");

        var command = new DeletePlan.Command(plan, user);

        await _sender.Send(command);

        return new Empty();
    }

    [AllowAnonymous]
    public override async Task<GetOnePlanRpc.Types.Response> GetOne(GetOnePlanRpc.Types.Request request,
        ServerCallContext context)
    {
        if (Guid.TryParse(request.PlanId, out var plan) is false)
            throw new ValidationException("Invalid guid");

        var command = new GetPlan.Request(plan);

        var response = await _sender.Send(command);

        return new GetOnePlanRpc.Types.Response
        {
            StoreId = response.StoreId.ToString(),
            PlanId = request.PlanId,
            Name = response.Name,
            Price = response.Price,
            IsFree = response.IsFree,
            Duration = response.Duration.ToDuration()
        };
    }

    [AllowAnonymous]
    public override async Task<GetManyPlansRpc.Types.Response> GetMany(GetManyPlansRpc.Types.Request request,
        ServerCallContext context)
    {
        if (Guid.TryParse(request.StoreId, out var store) is false)
            throw new ValidationException("Invalid guid");

        var command = new GetPlans.Request(store);

        var response = await _sender.Send(command, context.CancellationToken);

        var items = response.Select(m => new GetManyPlansRpc.Types.Response.Types.Item
        {
            PlanId = m.PlanId.ToString(),
            Name = m.Name,
            Price = m.Price,
            IsFree = m.IsFree,
            Duration = m.Duration.ToDuration()
        });

        return new GetManyPlansRpc.Types.Response
        {
            StoreId = request.StoreId,
            Items = { items }
        };
    }
}