using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public static class TestData
    {
        public static IEnumerable<object[]> GetPartnerNullAndInactiveData()
        {
            yield return new object[] { null };
            yield return new object[] { new Partner { IsActive = false } };
        }

        public static IEnumerable<object[]> GetPartnerWithActiveLimit()
        {
            yield return new object[] { FakeDataFactory.Partners[0] };
        }

        public static IEnumerable<object[]> GetPartnerWithInactiveLimit()
        {
            yield return new object[] { FakeDataFactory.Partners.Last() };
        }
    }
}
