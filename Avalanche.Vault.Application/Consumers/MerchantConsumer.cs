using Avalanche.Merchant.Events;
using Avalanche.Vault.Application.Features;
using DotNetCore.CAP;
using MediatR;

namespace Avalanche.Vault.Application.Consumers;

public sealed class MerchantConsumer : ICapSubscribe
{
    private readonly ISender _sender;

    public MerchantConsumer(ISender sender)
    {
        _sender = sender;
    }

    [CapSubscribe(nameof(OrderCompleted))]
    public async Task Consume(OrderCompleted message)
    {
        var command = new ExtendTicket.Command(message.StoreId, message.UserId, message.Availability,
            message.AvailabilitySpan);

        await _sender.Send(command);
    }
}