﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Zip.InstallmentsService.Entity.Dto;

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

        [JsonIgnore]
        public List<InstallmentDto> Installments { get; set; }
    }

}
