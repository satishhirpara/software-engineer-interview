using System;
using System.ComponentModel.DataAnnotations;

namespace Zip.InstallmentsService.Data.Models
{
    public class Installment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PaymentPlanId { get; set; }
        public PaymentPlan PaymentPlan { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
