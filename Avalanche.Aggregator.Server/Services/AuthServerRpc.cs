using Avalanche.Aggregator.Application;
using Avalanche.Aggregator.Application.Auth;
using Avalanche.Aggregator.Application.Domain;
using Avalanche.Drm.Auth;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Avalanche.Aggregator.Server.Services;

[Authorize]
public class AuthServerRpc : AuthService.AuthServiceBase
{
    private readonly AvalancheAggregatorContext _context;
    private readonly ChallengeManager _manager;

    public AuthServerRpc(AvalancheAggregatorContext context, ChallengeManager manager)
    {
        _context = context;
        _manager = manager;
    }

    public override async Task Connect(IAsyncStreamReader<ConnectRpc.Types.Command> requestStream,
        IServerStreamWriter<ConnectRpc.Types.Response> responseStream, ServerCallContext context)
    {
        var organization = context.RequestHeaders.Get("organization_id");

        if (organization is null)
            throw new ArgumentException("organization_id missing in request headers");

        var server = new Application.Domain.Server
        {
            OrganizationId = Guid.Parse(organization.Value)
        };

        var entry = await _context.Servers.AddAsync(server, cancellationToken: context.CancellationToken);

        await _context.SaveChangesAsync(cancellationToken: context.CancellationToken);

        var processor = _manager.Add(entry.Entity.Id);

        while (await requestStream.MoveNext())
        {
            var record = new Record
            {
                ServerId = entry.Entity.Id
            };

            await _context.Records.AddAsync(record, cancellationToken: context.CancellationToken);

            await _context.SaveChangesAsync(cancellationToken: context.CancellationToken);

            await processor.WaitForChallengeAsync(async result =>
            {
                if (result.CorrelationId == requestStream.Current.CorrelationId)
                {
                    record.UserId = result.UserId;
                    
                    record.Message = result.Message;
                    record.Succeed = result.Succeed;

                    _context.Records.Update(record);

                    await _context.SaveChangesAsync(cancellationToken: context.CancellationToken);

                    await responseStream.WriteAsync(new ConnectRpc.Types.Response
                    {
                        RecordId = result.RecordId.ToString(),
                        UserId = result.UserId.ToString(),

                        Success = result.Succeed,
                        Message = result.Message
                    });
                }
            });
        }

        _manager.Remove(entry.Entity.Id);
    }
}