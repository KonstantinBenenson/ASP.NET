using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions;

public interface IPublisher<TMessage>
{
    Task SendAsync(TMessage message, Uri uri, CancellationToken token);
    Task PublishAsync(TMessage message, CancellationToken token);
}
