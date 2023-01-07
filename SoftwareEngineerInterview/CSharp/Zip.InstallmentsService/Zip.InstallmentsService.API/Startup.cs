using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using Zip.InstallmentsService.API.Filter;
using Zip.InstallmentsService.API.Middleware;
using Zip.InstallmentsService.Core.Implementation;
using Zip.InstallmentsService.Core.Interface;
using Zip.InstallmentsService.Data.Interface;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Data.Repository;
using Zip.InstallmentsService.Entity.Common;
using Zip.InstallmentsService.Entity.V1.Request;

namespace Zip.InstallmentsService.Service
{
    /// <summary>
    /// Start up class mainly used to set up the configuration and middele wares
    /// </summary>
    public class Startup
    {
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Intialization in Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            //configure fluent validation
            services.AddControllers();
            services.AddMvc(options =>
            {
                options.Filters.Add(new ValidationFilter());
            }).AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<PaymentPlanRequest>();
            });

            //configure EF In Memory database
            services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("PaymentPlan"));

            //configure auto mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //configure swagger
            services.AddSwaggerGen();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Implement Swagger UI",
                    Description = "A simple example to Implement Swagger UI",
                });
            });

            //configure logging
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<ApplicationLog>>();
            services.AddSingleton(typeof(ILogger), logger);

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSetting>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSetting>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                };
            });

            //configure
            services.AddApplicationInsightsTelemetry();

            //configuration for dependancy injection
            services.AddScoped<IPaymentPlanProvider, PaymentPlanProvider>();
            services.AddScoped<IPaymentPlanRepository, PaymentPlanRepository>();
            services.AddScoped<IInstallmentProvider, InstallmentProvider>();

            //configure api controllers
            services.AddControllers().AddNewtonsoftJson();

            //configure api versioning to the project
            services.AddApiVersioning(x =>
                {
                    x.DefaultApiVersion = new ApiVersion(1, 0); //specify the default API version as 1.0
                    x.AssumeDefaultVersionWhenUnspecified = true; // use the default versin number if the client hasn't specified the API version in the request.

                    //x.ApiVersionReader = new MediaTypeApiVersionReader("version"); // add along with accept in header
                    //x.ApiVersionReader = new HeaderApiVersionReader("X-Version"); // own new key in header
                    x.ApiVersionReader = ApiVersionReader.Combine(
                        new MediaTypeApiVersionReader("version"),
                        new HeaderApiVersionReader("X-Version")
                        );

                    x.ReportApiVersions = true; //Advertise the api versions supported for particular endpoint
                });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApiContext context)
        {
            //Configure developer exception page for dev environment
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Configure swagger and swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Showing API V1");
            });

            app.UseHttpsRedirection();

            //Configure routing
            app.UseRouting();

            //Configure global cors policy
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            //Configure global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            //Configure authentication
            //app.UseAuthentication(); // UnComment for JWT token based authentication
            app.UseAuthorization();

            //Configure endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
