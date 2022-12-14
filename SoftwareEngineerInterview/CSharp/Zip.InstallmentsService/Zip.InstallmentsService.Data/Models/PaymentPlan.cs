using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zip.InstallmentsService.Data.Models
{
    public class PaymentPlan
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal PurchaseAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public List<Installment> Installments { get; set; }

    }
}
