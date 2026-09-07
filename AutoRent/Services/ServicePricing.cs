using System.Collections.Generic;
using AutoRent.Models;

namespace AutoRent.Services
{
    public static class ServicePricing
    {
        private static readonly Dictionary<VehicleCategory, decimal> PriceByCategory =
            new Dictionary<VehicleCategory, decimal>
            {
                [VehicleCategory.OsobnoVozilo] = 300M,
                [VehicleCategory.PutnickiKombi] = 550M,
                [VehicleCategory.TeretniKombi] = 475M,
                [VehicleCategory.Limuzina] = 950M
            };

        public static decimal GetFixedPrice(VehicleCategory category)
        {
            return PriceByCategory[category];
        }
    }
}
