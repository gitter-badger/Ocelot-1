using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ocelot.Library.Infrastructure.Repository;
using Ocelot.Library.Infrastructure.Responder;

namespace Ocelot.Library.Middleware
{
    public class HttpResponderMiddleware : OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpResponder _responder;
        private readonly IScopedRequestDataRepository _scopedRequestDataRepository;

        public HttpResponderMiddleware(RequestDelegate next, 
            IHttpResponder responder,
            IScopedRequestDataRepository scopedRequestDataRepository)
            :base(scopedRequestDataRepository)
        {
            _next = next;
            _responder = responder;
            _scopedRequestDataRepository = scopedRequestDataRepository;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);

            if (PipelineError())
            {
                //todo obviously this needs to be better...prob look at response errors
                // and make a decision i guess
                var errors = GetPipelineErrors();

                await _responder.CreateNotFoundResponse(context);
            }
            else
            {
                var response = _scopedRequestDataRepository.Get<HttpResponseMessage>("Response");

                await _responder.CreateResponse(context, response.Data);
            }
        }
    }
}