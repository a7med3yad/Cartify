using Cartify.Application.Services.Interfaces.Merchant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cartify.API.Controllers.MerchantControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IMerchantTransactionServices _transactionServices;
        public TransactionController(IMerchantTransactionServices _transactionServices)
        {
            this._transactionServices = _transactionServices;
        }
    }
}
