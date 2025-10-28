using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartify.Application.Contracts.ProductDtos
{
    public class UpdateProductDetailDto
    {
        public int ProductDetailId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int QuantityAvailable { get; set; }
        public List<ProductAttributeDto> Attributes { get; set; }
    }

}
