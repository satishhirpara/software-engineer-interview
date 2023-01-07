using System;

namespace Zip.InstallmentsService.Entity.Dto
{
    /// <summary>
    /// Data structure which defines all the properties for a Installment dto
    /// </summary>
    public class InstallmentDto
    {
        public InstallmentDto()
        { 
        
        }

        public InstallmentDto(Guid id, Guid paymentPlanId, DateTime? dueDate, decimal amount, DateTime createdOn, Guid createdBy) 
        {
            this.Id = id;
            this.PaymentPlanId = paymentPlanId;
            this.DueDate = dueDate?.Date ?? DateTime.MinValue;
            this.Amount = amount;
            this.CreatedOn = createdOn;
            this.CreatedBy = createdBy;
        }

        public Guid Id { get; set; }
        public Guid PaymentPlanId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }

    }
}
