using Dalisama.ResilientMicroserviceArchitecture.Common;
using Dalisama.ResilientMicroserviceArchitecture.Middleware.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Dalisama.ResilientMicroserviceArchitecture.Middleware.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MiddlewareController : ControllerBase
    {
        // GET: api/<MiddlewareController>



        // GET api/<MiddlewareController>/5
        [HttpGet]
        [ActionName("BigRedButton")]
        public void BigRedButton([FromServices] ServiceStatusMiddleware serviceStatusMiddleware)
        {
            serviceStatusMiddleware.RaiseServiceHealthDegadedEvent(new ServiceHealthEventArgs
            {
                Message = "health down",
                TimeReached = DateTime.UtcNow
            });
        }
        [HttpGet]
        [ActionName("BigGreenButton")]
        public void BigGreenButton([FromServices] ServiceStatusMiddleware serviceStatusMiddleware)
        {
            serviceStatusMiddleware.RaiseServiceHealthUpEvent(new ServiceHealthEventArgs
            {
                Message = "health Up",
                TimeReached = DateTime.UtcNow
            });
        }

        // POST api/<MiddlewareController>

    }
}
