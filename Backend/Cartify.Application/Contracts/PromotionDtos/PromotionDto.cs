using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartify.Application.Contracts.PromotionDtos
{
    public class PromotionDto
    {
        public string PromotionName { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}
