using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Zip.InstallmentsService.Entity.Dto;

namespace Zip.InstallmentsService.Entity.V1.Request
{
    public class PaymentPlanRequest
    {
    }

    /// <summary>
    /// Class which definess all properties of Create payment plan request
    /// </summary>
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

    /// <summary>
    /// Class which definess all validation rule of Create payment plan request
    /// </summary>
    public class CreatePaymentPlanRequestValidator : AbstractValidator<CreatePaymentPlanRequest>
    {
        public CreatePaymentPlanRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Please provide userid.");
            RuleFor(x => x.PurchaseAmount).GreaterThan(0).WithMessage("Please provide valid order amount.");
            RuleFor(x => x.NoOfInstallments).GreaterThan(0).WithMessage("Please provide valid no of installments.");
            RuleFor(x => x.PurchaseAmount).GreaterThan(y => y.NoOfInstallments).WithMessage("An order amount must be greater then no of installments.");
            RuleFor(x => x.FrequencyInDays).GreaterThan(0).LessThanOrEqualTo(365).WithMessage("Please provide valid frequency between 0 to 365 days.");
        }
    }


}
