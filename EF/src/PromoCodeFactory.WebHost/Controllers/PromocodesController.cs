using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Utils;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodeRepo;
        private readonly IExtendedRepository<Employee> _employeeRepo;
        private readonly IExtendedRepository<Customer> _customerRepo;
        private readonly IExtendedRepository<Preference> _preferenceRepo;


        public PromocodesController(IRepository<PromoCode> promoCodeRepo, IExtendedRepository<Employee> employeeRepo,
            IExtendedRepository<Customer> customerRepo, IExtendedRepository<Preference> preferenceRepo)
        {
            _promoCodeRepo = promoCodeRepo;
            _employeeRepo = employeeRepo;
            _customerRepo = customerRepo;
            _preferenceRepo = preferenceRepo;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync(CancellationToken token)
        {
            var promoCodes = await _promoCodeRepo.GetAllAsync(token); 
            if (!promoCodes.Any()) { return NotFound(); }
            return Ok(promoCodes.ToShortResponseList());
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с Предпочтением.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request, CancellationToken token)
        {
            var customersWithPreferences = await _customerRepo
                .GetByFilterAsync(x => x.CustomersPreferences.Any(cp => cp.Preference.Name == request.Preference), token);

            if (customersWithPreferences.Any())
            {
                var preference = await _preferenceRepo.GetByNameAsync(request.Preference, token);
                var employee = await _employeeRepo.GetByNameAsync(request.PartnerFirstName, token);

                foreach (var customer in customersWithPreferences)
                {
                    customer.PromoCodes ??= new List<PromoCode>();
                    var promo = new PromoCode
                    {
                        Code = request.PromoCode,
                        BeginDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(7),
                        ServiceInfo = request.ServiceInfo,
                        PartnerManagerId = employee.Id,
                        PartnerName = request.PartnerFirstName,
                        PreferenceId = preference.Id,
                    };
                    customer.PromoCodes.Add(promo);
                    await _promoCodeRepo.CreateAsync(promo, token);
                }
            }

            return NoContent();
        }
    }
}