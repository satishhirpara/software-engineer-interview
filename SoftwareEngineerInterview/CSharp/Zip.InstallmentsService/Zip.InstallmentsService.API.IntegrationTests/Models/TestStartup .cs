using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zip.InstallmentsService.API.Controllers.V1;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Service;

namespace Zip.InstallmentsService.API.IntegrationTests.Models
{
    internal class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddApplicationPart(typeof(PaymentPlanController).Assembly);
            services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("PaymentPlan"));
            base.ConfigureServices(services);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApiContext context)
        {
            base.Configure(app, env, context);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

    }
}
