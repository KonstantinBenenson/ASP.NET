using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.Common.EventModels;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Services.Consumers;

public class GivePromoCodeToCustomerConsumer : IConsumer<GivePromocodeToCustomerEvent>
{
    private readonly IRepository<PromoCode> _promoCodesRepository;
    private readonly IRepository<Preference> _preferencesRepository; 
    private readonly IRepository<Customer> _customersRepository;

    private readonly ILogger<GivePromoCodeToCustomerConsumer> _logger;

    public GivePromoCodeToCustomerConsumer(IRepository<Preference> preferencesRepository, IRepository<Customer> customersRepository, IRepository<PromoCode> promoCodesRepository, ILogger<GivePromoCodeToCustomerConsumer> logger)
    {
        _preferencesRepository = preferencesRepository;
        _customersRepository = customersRepository;
        _promoCodesRepository = promoCodesRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GivePromocodeToCustomerEvent> context)
    {
        try
        {
            _logger.LogInformation($"Message received with MessageId: '{context.MessageId}'.");
            var message = context.Message;
            ArgumentNullException.ThrowIfNull(message.PreferenceId);

            _logger.LogInformation($"Retrieving a Preference from DB by Id: '{message.PreferenceId}'.");
            //Получаем предпочтение по имени
            var preference = await _preferencesRepository.GetByIdAsync(message.PreferenceId);

            if (preference == null)
            {
                _logger.LogInformation($"Could not find the Preference by the given Id. Exiting.");
                return;
            }

            _logger.LogInformation($"Retrieving a list of Custromers, who has a Preference {preference.Name}.");
            //  Получаем клиентов с этим предпочтением:
            var customers = await _customersRepository
                .GetWhere(d => d.Preferences.Any(x =>
                    x.Preference.Id == preference.Id));

            var customersCount = customers.Count();
            if (customersCount == 0)
            {
                _logger.LogInformation($"No Customers has the Preference. Exiting.");
                return;
            }

            _logger.LogInformation($"Giving the PromoCode to each found Customer: {customersCount}.");
            PromoCode promoCode = MapFromModel(message, preference, customers);
            await _promoCodesRepository.AddAsync(promoCode);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error has occured while processing a message from queue: '{ex.Message}'.");
        }
    }

    private static PromoCode MapFromModel(GivePromocodeToCustomerEvent request, Preference preference, IEnumerable<Customer> customers)
    {
        var promocode = new PromoCode();
        promocode.Id = request.PromoCodeId;

        promocode.PartnerId = request.PartnerId;
        promocode.Code = request.PromoCode;
        promocode.ServiceInfo = request.ServiceInfo;

        promocode.BeginDate = DateTime.Parse(request.BeginDate);
        promocode.EndDate = DateTime.Parse(request.EndDate);

        promocode.Preference = preference;
        promocode.PreferenceId = preference.Id;

        promocode.Customers = new List<PromoCodeCustomer>();

        foreach (var item in customers)
        {
            promocode.Customers.Add(new PromoCodeCustomer()
            {
                CustomerId = item.Id,
                Customer = item,
                PromoCodeId = promocode.Id,
                PromoCode = promocode
            });
        };

        return promocode;
    }
}
