using System;

namespace Zip.InstallmentsService.Entity.Dto
{
    /// <summary>
    /// Data structure which defines all the properties for a Installment dto
    /// </summary>
    public class InstallmentDto
    {
        public Guid Id { get; set; }
        public Guid PaymentPlanId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }

    }
}
