using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartify.Application.Contracts.ProfileDtos
{
    public class UpdateStoreInfoDto
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
    }
}
