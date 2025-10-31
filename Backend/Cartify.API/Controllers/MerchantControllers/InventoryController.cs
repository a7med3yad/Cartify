using Cartify.Application.Services.Interfaces.Merchant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cartify.API.Controllers.MerchantControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IMerchantInventoryServices _inventoryServices;
        public InventoryController(IMerchantInventoryServices _inventoryServices)
        {
            this._inventoryServices = _inventoryServices;
        }
    }
}
