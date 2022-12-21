using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Zip.InstallmentsService.Entity.Dto;

namespace Zip.InstallmentsService.Entity.V1.Response
{
    public class PaymentPlanResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal PurchaseAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public List<InstallmentResponse> Installments { get; set; }
    }
}
