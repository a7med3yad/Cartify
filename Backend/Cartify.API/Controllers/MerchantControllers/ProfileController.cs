using Cartify.Application.Services.Interfaces.Merchant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cartify.API.Controllers.MerchantControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IMerchantProfileServices _profileServices;
        public ProfileController(IMerchantProfileServices _profileServices)
        {
            this._profileServices = _profileServices;
        }
    }
}
