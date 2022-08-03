using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.UploadControllers
{
	[Produces("application/json")]
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/master/upload-budget-currencies")]
	public class BudgetCurrenciesUploadController : BasicUploadController<BudgetCurrencyService, BudgetCurrency, BudgetCurrencyViewModel, BudgetCurrencyService.BudgetCurrencyMap, CoreDbContext>
	{
		private static readonly string ApiVersion = "1.0";

		public BudgetCurrenciesUploadController(BudgetCurrencyService service) : base(service, ApiVersion)
		{

		}
	}
}
