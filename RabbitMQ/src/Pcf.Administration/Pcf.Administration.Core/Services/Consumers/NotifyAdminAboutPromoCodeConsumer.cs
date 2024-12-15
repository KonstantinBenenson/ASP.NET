using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.Core.Domain.Events;
using System;
using System.Threading.Tasks;

namespace Pcf.Administration.Core.Services.Consumers;

public class NotifyAdminAboutPromoCodeConsumer : IConsumer<NotifyAdminAboutPartnerManagerPromoCodeEvent>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly ILogger<NotifyAdminAboutPromoCodeConsumer> _logger;

    public NotifyAdminAboutPromoCodeConsumer(IRepository<Employee> employeeRepository, ILogger<NotifyAdminAboutPromoCodeConsumer> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NotifyAdminAboutPartnerManagerPromoCodeEvent> context)
    {
        try
        {
            _logger.LogInformation($"Message received with MessageId: '{context.MessageId}'.");
            var message = context.Message;
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNull(message.PartnerManagerId);

            _logger.LogInformation($"Retrieving an Employee from DB by Id: '{message.PartnerManagerId}'.");
            var employee = await _employeeRepository.GetByIdAsync(message.PartnerManagerId);

            if (employee == null)
            {
                _logger.LogInformation($"Could not find the Employee by the given Id. Exiting.");
                return;
            }

            employee.AppliedPromocodesCount++;
            _logger.LogInformation($"Increasing promocodes count by 1 to '{employee.AppliedPromocodesCount}'");

            await _employeeRepository.UpdateAsync(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error has occured while processing a message from queue: '{ex.Message}'.");
        }
        
    }
}
