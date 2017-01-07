using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using postfix.Options;
using AutoMapper;
using postfix.ViewModels;
using postfix.Models;

namespace postfix
{
    public partial class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;
        SymmetricSecurityKey _signingKey;

        public Startup(IHostingEnvironment env) {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);
            services.AddOptions();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials() );
            });

            // Use policy auth.
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "PostfixAdmins",
                    policy => policy.RequireClaim("PostfixUserLevel", "1")
                );
            });

            // Get options from app settings
            var jwtAppSettingOptions = _config.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            string secretKey = _config["Keys:JwtKey"];
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            // Make authentication compulsory across the board (i.e. shut
            // down EVERYTHING unless explicitly opened up).
            services.AddMvc(config => {
                    if (_env.IsProduction()) {
                        config.Filters.Add(new RequireHttpsAttribute());
                    }

                    var policy = new AuthorizationPolicyBuilder()
                                        .RequireAuthenticatedUser()
                                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddJsonOptions(config =>
                {
                    config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });      // set up service for di container for Services

            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddDebug(LogLevel.Information);
            }
            else
            {
                loggerFactory.AddDebug(LogLevel.Error);
            }

            app.UseCors("CorsPolicy");

            app.UseDefaultFiles();
            app.UseStaticFiles();

            var jwtAppSettingOptions = _config.GetSection(nameof(JwtIssuerOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            Mapper.Initialize(config =>
            {
                config
                    .CreateMap<ExecStackViewModel, ExecutionStack>()
                    .ReverseMap();
                
                config
                    .CreateMap<StackItemViewModel, StackItem>()
                    .ReverseMap();
            });

            app.UseMvc(config => {
                config.MapRoute(
                    name: "Entry Point",
                    template: "api",
                    defaults: new { controller = "EntryPoint", action = "Get" }
                );
            });

        }
    }
}
