﻿using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ocelot.Library.Infrastructure.Responder
{
    public interface IHttpResponder
    {
        Task<HttpContext> CreateResponse(HttpContext context, HttpResponseMessage response);
        Task<HttpContext> CreateNotFoundResponse(HttpContext context);

    }
}
