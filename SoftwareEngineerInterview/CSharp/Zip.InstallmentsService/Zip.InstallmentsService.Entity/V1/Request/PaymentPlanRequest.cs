using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations;

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
    }

    /// <summary>
    /// Class which definess all validation rule of Create payment plan request
    /// </summary>
    public class CreatePaymentPlanRequestValidator : AbstractValidator<CreatePaymentPlanRequest>
    {
        public CreatePaymentPlanRequestValidator()
        {
            RuleFor(x => x.UserId).NotNull();
            RuleFor(x => x.PurchaseAmount).GreaterThan(0);
            RuleFor(x => x.NoOfInstallments).GreaterThan(0);
            RuleFor(x => x.PurchaseAmount).GreaterThan(y => y.NoOfInstallments);
            RuleFor(x => x.FrequencyInDays).GreaterThan(0).LessThanOrEqualTo(365);
        }
    }


}
