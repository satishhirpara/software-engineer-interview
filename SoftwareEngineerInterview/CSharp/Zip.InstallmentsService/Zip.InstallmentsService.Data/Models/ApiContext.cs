using Microsoft.EntityFrameworkCore;

namespace Zip.InstallmentsService.Data.Models
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
        }

        public DbSet<PaymentPlan> PaymentPlans { get; set; }

        public DbSet<Installment> Installments { get; set; }
    }
}
