﻿using System.Text.RegularExpressions;
using Ocelot.Library.Infrastructure.Responses;

namespace Ocelot.Library.Infrastructure.UrlMatcher
{
    public class RegExUrlMatcher : IUrlPathToUrlTemplateMatcher
    {
        public Response<UrlMatch> Match(string upstreamUrlPath, string upstreamUrlPathTemplate)
        {
            var regex = new Regex(upstreamUrlPathTemplate);

            return regex.IsMatch(upstreamUrlPath) 
                ? new OkResponse<UrlMatch>(new UrlMatch(true)) 
                : new OkResponse<UrlMatch>(new UrlMatch(false));
        }
    }
}
