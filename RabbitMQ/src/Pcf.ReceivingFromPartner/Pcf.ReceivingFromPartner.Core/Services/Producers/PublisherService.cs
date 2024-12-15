using MassTransit;
using Pcf.ReceivingFromPartner.Core.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Services.Producers;

public class PublisherService<TMessage> : IPublisher<TMessage> where TMessage : class
{
    private readonly ISendEndpointProvider _provider;
    private readonly IPublishEndpoint _publishEndpoint;

    public PublisherService(ISendEndpointProvider provider, IPublishEndpoint publishEndpoint)
    {
        _provider = provider;
        _publishEndpoint = publishEndpoint;
    }

    public async Task SendAsync(TMessage message, Uri uri, CancellationToken token = default)
    {
        var endpoint = await _provider.GetSendEndpoint(uri);
        await endpoint.Send(message, token);
    }

    public async Task PublishAsync(TMessage message, CancellationToken token = default)
    {
        await _publishEndpoint.Publish(message, token);
    }
}
