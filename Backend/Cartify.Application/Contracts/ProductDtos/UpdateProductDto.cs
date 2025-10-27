using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartify.Application.Contracts.ProductDtos
{
    public class UpdateProductDto
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int TypeId { get; set; }
    }
}
