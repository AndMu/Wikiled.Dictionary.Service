using System;
using System.Reflection;
using System.Runtime.Caching;
using System.Xml.Linq;
using AspNetCoreRateLimit;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using Polly;
using StackExchange.Redis;
using Wikiled.Core.Utility.Cache;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Dictionary.Logic;
using Wikiled.Postal.Logic;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic.Pool;
using Wikiled.Service.Middleware;
using Wikiled.Service.Shared.Contact;

namespace Wikiled.Service
{
    public class Startup
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            env.ConfigureNLog("nlog.config");
            logger.Debug($"Starting: {Assembly.GetExecutingAssembly().GetName().Version}");
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors("CorsPolicy");

            // add NLog to ASP.NET Core
            loggerFactory.AddNLog();

            // add NLog.Web
            app.UseMiddleware<ErrorWrappingMiddleware>();
            app.AddNLogWeb();
            app.UseIpRateLimiting();
            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Needed to add this section, and....
            services.AddCors(
                options =>
                    {
                        options.AddPolicy(
                            "CorsPolicy",
                            itemBuider => itemBuider.AllowAnyOrigin()
                                                    .AllowAnyMethod()
                                                    .AllowAnyHeader()
                                                    .AllowCredentials());
                    });

            // Add framework services.
            services.AddMvc(
                options => { });

            // needed to load configuration from appsettings.json
            services.AddOptions();

            // Create the container builder.
            var builder = new ContainerBuilder();
            ConfigureLimiter(builder, services);

            // Register dependencies, populate the services from
            // the collection, and build the container. If you want
            // to dispose of the container at the end of the app,
            // be sure to keep a reference to it as a property or field.
            // builder.RegisterType<MyType>().As<IMyType>();
            SetupRedisPersistency(builder);
            builder.RegisterType<ContactManager>()
                   .As<IContactManager>();
            builder.RegisterType<RedisLanguageDictionary>()
                   .As<ILanguageDictionary>()
                   .SingleInstance();
            builder.RegisterType<PostalManager>()
                   .As<IPostalManager>()
                   .SingleInstance();

            builder.Populate(services);
            var appContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(appContainer);
        }

        private static void SetupRedisPersistency(ContainerBuilder builder)
        {
            var doc = XDocument.Load("redis.xml");
            var config = doc.XmlDeserialize<RedisConfiguration[]>();
            foreach (var redisConfiguration in config)
            {
                builder.RegisterInstance(redisConfiguration);
            }

            var startupPolicy = Policy
                .Handle<RedisConnectionException>(
                    exception =>
                        {
                            logger.Error(exception);
                            return true;
                        })
                .WaitAndRetry(
                    3,
                    i =>
                        {
                            logger.Warn("Connection to REDIS failed. Waiting to retry");
                            return TimeSpan.FromMinutes(i);
                        });

            builder.RegisterInstance<ICacheHandler>(new RuntimeCache(new MemoryCache("Local"), TimeSpan.FromMinutes(1)));
            builder.RegisterType<RedisLinksPool>()
                   .As<IRedisLinksPool>()
                   .OnActivating(item => startupPolicy.Execute(() => item.Instance.Open()))
                   .SingleInstance();
        }

        private void ConfigureLimiter(ContainerBuilder builder, IServiceCollection services)
        {
            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();

            // load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            // load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            builder.RegisterType<MemoryCacheIpPolicyStore>()
                   .As<IIpPolicyStore>()
                   .SingleInstance();

            builder.RegisterType<MemoryCacheRateLimitCounterStore>()
                   .As<IRateLimitCounterStore>()
                   .SingleInstance();
        }
    }
}
