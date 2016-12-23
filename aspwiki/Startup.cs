using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ASPWiki.Services;
using ASPWiki.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.IO;
using ASPWiki.Services.Generators;
using AutoMapper;
using ASPWiki.Mapping;
using ASPWiki.Infastructure;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Twitter;

namespace ASPWiki
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            if (!Directory.Exists(Constants.LoggingFilePath))
            {
                Directory.CreateDirectory(Constants.LoggingFilePath);
            }

            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.File(Constants.LoggingFilePath + "/" + Constants.LoggingFileName).CreateLogger();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //ApplicationInsightsTelemetry
            services.AddApplicationInsightsTelemetry(Configuration);

            //AddConfigurationOptions
            services.Configure<ConfigurationOptions>(Configuration.GetSection(Constants.ENV_VARIABLE_PREFIX));

            AutoMapperConfiguration.Configure();
            services.AddSingleton(Mapper.Instance);

            services.AddSession();

            services.AddMvc();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IDatabaseConnection, DatabaseConnection>();

            services.AddSingleton<IRouteGenerator, RouteGenerator>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            services.AddSingleton<IAuthorizationHandler, AuthenticationRequirementHandler>();
            services.AddSingleton<IAuthorizationHandler, AnonymousRequirementHandler>();

            services.AddSingleton<IWikiRepository, WikiRepository>();
            services.AddSingleton<IWikiService, WikiService>();

            //TEST ENVIRONMENT TESTDATA GENERATION
            services.AddSingleton<IGarbageGenerator<Attachment>, AttachmentGenerator>();
            services.AddSingleton<IGarbageGenerator<WikiPage>, WikiPageGenerator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime,
            IDatabaseConnection databaseConnection, IWikiRepository wikiRepo, IGarbageGenerator<WikiPage> wikiPageGenerator, IOptions<ConfigurationOptions> options, IAuthenticationService authenticationService)
        {

            //TEST DATA
            if (env.IsDevelopment())
            {
                var testDataGenerator = new TestDataGenerator(databaseConnection, wikiRepo, wikiPageGenerator);
            }

            //LOGGING
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            // Ensure any buffered events are sent at shutdown
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
      
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();

                app.UseApplicationInsightsRequestTelemetry();
                app.UseApplicationInsightsExceptionTelemetry();
            }
            else
            {
                app.UseExceptionHandler("/Wiki/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseStatusCodePagesWithReExecute("/Wiki/Error/{0}");

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = Constants.AuthenticationSchemeCookies,
                LoginPath = new PathString("/Wiki/Error"),
                AccessDeniedPath = new PathString("/Wiki/Forbidden"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseTwitterAuthentication(new TwitterOptions
            {
                ConsumerKey = options.Value.TwitterKey,
                ConsumerSecret = options.Value.TwitterKeySecret,
                SignInScheme = Constants.AuthenticationSchemeCookies,
                RetrieveUserDetails = false,
                SaveTokens = true,
                Events = new TwitterEvents()
                {
                    OnCreatingTicket = authenticationService.OnCreateTicket,
                    OnRemoteFailure = authenticationService.OnRemoteFailure
                }
            });

            //Login Map
            app.Map("/login", signoutApp =>
            {
                signoutApp.Run(async context =>
                {
                    if (env.IsDevelopment())
                    {
                        await authenticationService.CreateDevClaim();
                        context.Response.Redirect(Constants.LoginRedirectRoute);
                    }
                    else
                    {
                        await context.Authentication.ChallengeAsync(Constants.AuthenticationSchemeKey, authenticationService.GetAuthenticationProperties());
                        return;
                    }
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    "View",
                    template: "{controller=Wiki}/{action=View}/{*path}");
            });
        }
    }
}
