using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Operations.AnaliticOperations;

namespace ETLService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ApiAnaliticController : ControllerBase
    {
        [HttpGet]
        public ActionResult Absentismo(DateTime desde, DateTime hasta)
        {
            return Ok(AnaliticAbsentismoOperation.GetByPeriodo(desde, hasta));
        }        
    }
}