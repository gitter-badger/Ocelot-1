﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.Library.Infrastructure.Configuration;
using Ocelot.Library.Infrastructure.Configuration.Yaml;
using Ocelot.Library.Infrastructure.DownstreamRouteFinder;
using Ocelot.Library.Infrastructure.Repository;
using Ocelot.Library.Infrastructure.RequestBuilder;
using Ocelot.Library.Infrastructure.Requester;
using Ocelot.Library.Infrastructure.Responder;
using Ocelot.Library.Infrastructure.UrlMatcher;
using Ocelot.Library.Infrastructure.UrlTemplateReplacer;
using Ocelot.Library.Middleware;

namespace Ocelot
{
    using Library.Infrastructure.Authentication;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddYamlFile("configuration.yaml")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
                        
            services.Configure<YamlConfiguration>(Configuration);

            // Add framework services.
            services.AddSingleton<IOcelotConfiguration, OcelotConfiguration>();
            services.AddSingleton<IUrlPathToUrlTemplateMatcher, RegExUrlMatcher>();
            services.AddSingleton<ITemplateVariableNameAndValueFinder, TemplateVariableNameAndValueFinder>();
            services.AddSingleton<IDownstreamUrlTemplateVariableReplacer, DownstreamUrlTemplateVariableReplacer>();
            services.AddSingleton<IDownstreamRouteFinder, DownstreamRouteFinder>();
            services.AddSingleton<IHttpRequester, HttpClientHttpRequester>();
            services.AddSingleton<IHttpResponder, HttpContextResponder>();
            services.AddSingleton<IRequestBuilder, HttpRequestBuilder>();
            services.AddSingleton<IRouteRequiresAuthentication, RouteRequiresAuthentication>();

            // see this for why we register this as singleton http://stackoverflow.com/questions/37371264/invalidoperationexception-unable-to-resolve-service-for-type-microsoft-aspnetc
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IScopedRequestDataRepository, ScopedRequestDataRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            loggerFactory.AddDebug();

            app.UseHttpResponderMiddleware();

            app.UseDownstreamRouteFinderMiddleware();

            app.UseAuthenticationMiddleware();

            app.UseDownstreamUrlCreatorMiddleware();

            app.UseHttpRequestBuilderMiddleware();

            app.UseHttpRequesterMiddleware();
        }
    }
}
