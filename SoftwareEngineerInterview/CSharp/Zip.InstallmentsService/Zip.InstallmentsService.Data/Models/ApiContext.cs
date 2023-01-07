using Microsoft.EntityFrameworkCore;

namespace Zip.InstallmentsService.Data.Models
{
    /// <summary>
    /// ApiContext for EF related operations
    /// </summary>
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// payment plans
        /// </summary>
        public DbSet<PaymentPlan> PaymentPlans { get; set; }

        /// <summary>
        /// installments
        /// </summary>
        public DbSet<Installment> Installments { get; set; }
    }
}
