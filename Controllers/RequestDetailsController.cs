using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace ServerDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestDetailsController : ControllerBase
    {
        public IActionResult Get()
        {
            var tlsInfo = HttpContext.Features.Get<ITlsHandshakeFeature>();
            var requestDetails = new
            {
                Request.Protocol,
                Request.Method,
                Url = Request.GetDisplayUrl(),
                RawUrl = HttpContext.Features.Get<IHttpRequestFeature>().RawTarget,
                Headers = Request.Headers.ToList(),
                Tls = tlsInfo ?? null,
            };
            return Ok(requestDetails);
        }
    }
}