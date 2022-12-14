using System;
using System.ComponentModel.DataAnnotations;

namespace Zip.InstallmentsService.Entity.V1.Request
{
    public class PaymentPlanRequest
    {
    }

    public class CreatePaymentPlanRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal PurchaseAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int NoOfInstallments { get; set; }
        public int FrequencyInDays { get; set; }
    }

}
