using AutoMapper;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Entity.V1.Request;
using Zip.InstallmentsService.Entity.V1.Response;

namespace Zip.InstallmentsService.Core.Mapping
{
    /// <summary>
    /// Created profiles for AutoMapper (Mapping of objects) of entities
    /// </summary>
    public class PaymentPlanProfile : Profile
    {
        public PaymentPlanProfile()
        {
            CreateMap<CreatePaymentPlanRequest, PaymentPlan>();
            CreateMap<PaymentPlan, PaymentPlanResponse>();
        }

    }
}
