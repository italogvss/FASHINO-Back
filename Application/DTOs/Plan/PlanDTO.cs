using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Plan
{
    public class PlanDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Features { get; set; }
        public ICollection<PlanPriceDTO> Prices { get; set; }

    }

    public class PlanPriceDTO
    {
        public string Region { get; set; }
        public string Currency { get; set; }
        public decimal PriceMonthly { get; set; }
        public decimal PriceYearly { get; set; }
    }

    public class PlanResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Features { get; set; }
        public string Currency { get; set; }
        public decimal PriceMonthly { get; set; }
        public decimal PriceYearly { get; set; }

    }
}
