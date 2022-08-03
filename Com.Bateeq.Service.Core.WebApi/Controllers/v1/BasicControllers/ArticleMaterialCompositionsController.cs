using System;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{

    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/articles/materialCompositions")]
    public class ArticleMaterialCompositionsController : BasicController<ArticleMaterialCompositionService, ArticleMaterialComposition, ArticleMaterialCompositionViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";
        public ArticleMaterialCompositionsController(ArticleMaterialCompositionService service) : base(service, ApiVersion)
        {
        }

    }
}
