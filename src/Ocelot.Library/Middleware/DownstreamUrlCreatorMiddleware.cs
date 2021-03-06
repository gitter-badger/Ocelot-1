using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ocelot.Library.Infrastructure.DownstreamRouteFinder;
using Ocelot.Library.Infrastructure.Repository;
using Ocelot.Library.Infrastructure.UrlTemplateReplacer;

namespace Ocelot.Library.Middleware
{
    public class DownstreamUrlCreatorMiddleware : OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDownstreamUrlTemplateVariableReplacer _urlReplacer;
        private readonly IScopedRequestDataRepository _scopedRequestDataRepository;

        public DownstreamUrlCreatorMiddleware(RequestDelegate next, 
            IDownstreamUrlTemplateVariableReplacer urlReplacer,
            IScopedRequestDataRepository scopedRequestDataRepository)
            :base(scopedRequestDataRepository)
        {
            _next = next;
            _urlReplacer = urlReplacer;
            _scopedRequestDataRepository = scopedRequestDataRepository;
        }

        public async Task Invoke(HttpContext context)
        {
            var downstreamRoute = _scopedRequestDataRepository.Get<DownstreamRoute>("DownstreamRoute");

            if (downstreamRoute.IsError)
            {
                SetPipelineError(downstreamRoute.Errors);
                return;
            }

            var downstreamUrl = _urlReplacer.ReplaceTemplateVariables(downstreamRoute.Data);

            if (downstreamUrl.IsError)
            {
                SetPipelineError(downstreamUrl.Errors);
                return;
            }

            _scopedRequestDataRepository.Add("DownstreamUrl", downstreamUrl.Data);
                
            await _next.Invoke(context);
        }
    }
}