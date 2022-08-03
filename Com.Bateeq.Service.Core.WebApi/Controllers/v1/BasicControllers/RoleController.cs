//using Com.DanLiris.Service.Core.Lib;
//using Com.DanLiris.Service.Core.Lib.Models.Account_and_Roles;
//using Com.DanLiris.Service.Core.Lib.Services.Account_and_Roles;
//using Com.DanLiris.Service.Core.Lib.ViewModels.Account_and_Roles;
//using Com.DanLiris.Service.Core.WebApi.Helpers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
//{
//    [Produces("application/json")]
//    [ApiVersion("1.0")]
//    [Route("v{version:apiVersion}/roles")]
//    public class RoleController : BasicController<RolesService, Role, RoleViewModel, CoreDbContext>
//    {
//        private new static readonly string ApiVersion = "1.0";
//        public RoleController(RolesService service) : base(service, ApiVersion)
//        {
//        }
//    }
//}
