using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IRepository<Employee> _employeeRepo;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<Preference> _preferenceRepo;


        public PromocodesController(IRepository<PromoCode> promoCodeRepo, IRepository<Employee> employeeRepo, 
            IRepository<Customer> customerRepo, IRepository<Preference> preferenceRepo)
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
            return Ok(promoCodes.ToShortResponseList());
        }

        /// <summary>
        /// Создать промокод и выдать его определенному клиенту.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(Guid id, GivePromoCodeRequest request, CancellationToken token)
        {
            var preference = await _preferenceRepo.GetByNameAsync(request.Preference, token);
            var employee = await _employeeRepo.GetByNameAsync(request.PartnerFirstName, token);

            var promo = new PromoCode
            {
                Id = Guid.NewGuid(),
                Code = request.PromoCode,
                BeginDate = DateTime.UtcNow,
                ServiceInfo = request.ServiceInfo,
                PartnerManagerId = employee.Id,
                PartnerName = request.PartnerFirstName,
                PreferenceId = preference.Id,
                CustomerId = id,
            };

            await _promoCodeRepo.CreateAsync(promo, token);
            return Created();
        }
    }
}