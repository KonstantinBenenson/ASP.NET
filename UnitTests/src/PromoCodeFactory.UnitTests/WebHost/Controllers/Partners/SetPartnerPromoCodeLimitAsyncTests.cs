using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.TestFixture;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests : IClassFixture<PartnerTestsFixture>
    {
        private PartnersController _partnerController;
        private PartnerTestsFixture _partnerTestsFixture;
        public SetPartnerPromoCodeLimitAsyncTests(PartnerTestsFixture partnerTestsFixture)
        {
            _partnerTestsFixture = partnerTestsFixture;
        }

        // Here I use a Moq as en example of how it can be used, when we need to make some related method return what we need. 
        // Later I use real Repository and seeded Data.
        [Theory]
        [MemberData(nameof(TestData.GetPartnerNullAndInactiveData), MemberType = typeof(TestData))]
        public async Task GetPartner_WhenIsNotFoundOrIsInactive_ReturnsError(Partner partner)
        {
            // Arrange 
            var partnersRepoMoq = new Mock<IRepository<Partner>>();
            partnersRepoMoq.Setup(c => c.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(partner);
            _partnerController = new(partnersRepoMoq.Object);
            var newLimit = 40;
            var request = new SetPartnerPromoCodeLimitRequest { Limit = newLimit, EndDate = DateTime.UtcNow.AddDays(3) };

            // Act
            var result = await _partnerController.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), request);

            // Assert
            result.Should().NotBeNull();
            if (partner is null)
                result.Should().BeOfType<NotFoundResult>();
            else
                result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [MemberData(nameof(TestData.GetPartnerWithActiveLimit), MemberType = typeof(TestData))]
        public async Task SetLimit_AddLimitToActivePartner_LimitIsAdded(Partner partner)
        {
            // Arrange 
            var partnerId = partner.Id;
            var partnersController = _partnerTestsFixture.PartnersController;
            var newLimit = new Random().Next(101, 1999);
            var request = new SetPartnerPromoCodeLimitRequest { Limit = newLimit, EndDate = DateTime.UtcNow.AddDays(3) };

            // Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);
            var partnerAfterUpdate = await _partnerTestsFixture.PartnersRepository.GetByIdAsync(partnerId);
            var partnerLimit = partnerAfterUpdate.PartnerLimits.LastOrDefault();

            // Assert
            partnerLimit.Should().NotBeNull();
            partnerLimit.Limit.Should().Be(newLimit);
        }

        [Theory]
        [MemberData(nameof(TestData.GetPartnerWithActiveLimit), MemberType = typeof(TestData))]
        public async Task SetLimit_AddLimitToActivePartner_LimitIsMoreThan_0(Partner partner)
        {
            // Arrange 
            var partnerId = partner.Id;
            var partnersController = _partnerTestsFixture.PartnersController;
            var newLimit = new Random().Next(101, 1999);
            var request = new SetPartnerPromoCodeLimitRequest { Limit = newLimit, EndDate = DateTime.UtcNow.AddDays(3) };

            // Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);
            var partnerAfterUpdate = await _partnerTestsFixture.PartnersRepository.GetByIdAsync(partnerId);
            var partnerLimit = partnerAfterUpdate.PartnerLimits.LastOrDefault();

            // Assert
            partnerLimit.Limit.Should().BeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestData.GetPartnerWithActiveLimit), MemberType = typeof(TestData))]
        public async Task SetLimit_PartnerWithActiveLimits_PromoCodesCountBecomes_0(Partner partner)
        {
            // Arrange 
            var partnerId = partner.Id;
            var partnersController = _partnerTestsFixture.PartnersController;
            var newLimit = 80;
            var request = new SetPartnerPromoCodeLimitRequest { Limit = newLimit, EndDate = DateTime.UtcNow.AddDays(3) };

            // Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);
            var partnerAfterUpdate = await _partnerTestsFixture.PartnersRepository.GetByIdAsync(partnerId);
            var promoCodes = partnerAfterUpdate.NumberIssuedPromoCodes;
            // Take the second from the end limit, which should had been active before we added a new one
            var prevLimit = (partnerAfterUpdate.PartnerLimits as List<PartnerPromoCodeLimit>)[partnerAfterUpdate.PartnerLimits.Count-2];
            var limit = partnerAfterUpdate.PartnerLimits.LastOrDefault()?.Limit;

            // Assert
            prevLimit.CancelDate.Should().NotBeNull(); // checking the previous Limit CancelDate was set
            limit.Should().Be(newLimit); // Limit now is renewed
            promoCodes.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(TestData.GetPartnerWithInactiveLimit), MemberType = typeof(TestData))]
        public async Task SetLimit_NoActiveLimits_PromoCodesCountIsNotChanged(Partner partner)
        {
            // Arrange 
            var partnerId = partner.Id;
            var partnersController = _partnerTestsFixture.PartnersController;
            var initialNumberOfPromoCodes = partner.NumberIssuedPromoCodes;
            var newLimit = 120;
            var request = new SetPartnerPromoCodeLimitRequest { Limit = newLimit, EndDate = DateTime.UtcNow.AddDays(3) };
            var partnerBeforeUpdate = await _partnerTestsFixture.PartnersRepository.GetByIdAsync(partnerId);

            // Act
            var result = await partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);
            var partnerAfterUpdate = await _partnerTestsFixture.PartnersRepository.GetByIdAsync(partnerId);
            var promoCodesAfterUpdate = partnerAfterUpdate.NumberIssuedPromoCodes;

            // Assert
            promoCodesAfterUpdate.Should().Be(initialNumberOfPromoCodes);
        }
    }
}