using System;
using System.Collections.Generic;

namespace Zip.InstallmentsService.Entity.Dto
{
    /// <summary>
    /// Data structure which defines all the properties for a PaymentPlan dto
    /// </summary>
    public class PaymentPlanDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal PurchaseAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public List<InstallmentDto> Installments { get; set; }

    }

}
