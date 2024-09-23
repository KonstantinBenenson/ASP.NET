using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

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
        private readonly IRepository<PromoCode> _repo;

        public PromocodesController(IRepository<PromoCode> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync(CancellationToken token)
        {
            var promoCodes = await _repo.GetAllAsync(token);
            var shortResponseList = promoCodes.Select(x => new PromoCodeShortResponse
            {
                Id = x.Id,
                Code = x.Code,
                ServiceInfo = x.ServiceInfo,
                PartnerName = x.PartnerName,
                BeginDate = x.BeginDate.ToShortDateString(),
                EndDate = x.EndDate.ToShortDateString(),
            }).ToList();

            return Ok(shortResponseList);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request, CancellationToken token)
        {
            var promo = new PromoCode 
            { 
                Id = Guid.NewGuid(),
                Code = request.PromoCode,
                BeginDate = DateTime.UtcNow,
                ServiceInfo = request.ServiceInfo,
                PartnerManagerId = Guid.NewGuid(),
                PartnerName = request.PartnerName,
                Preference = new Preference { Name = request.Preference }
            };

            await _repo.CreateAsync(promo, token);
            return Ok(promo);
        }
    }
}