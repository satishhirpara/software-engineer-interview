using AutoMapper;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Entity.V1.Request;
using Zip.InstallmentsService.Entity.V1.Response;

namespace Zip.InstallmentsService.Core.Mapping
{
    /// <summary>
    /// Class which defines all the mapped objects for payment plan
    /// </summary>
    public class PaymentPlanProfile : Profile
    {
        /// <summary>
        /// Set up a profile along with object mapping
        /// </summary>
        public PaymentPlanProfile()
        {
            CreateMap<CreatePaymentPlanRequest, PaymentPlan>();

            CreateMap<PaymentPlanDto, PaymentPlanResponse>();
            CreateMap<InstallmentDto, InstallmentResponse>();

            CreateMap<InstallmentResponse, Installment>();
            
        }

    }
}
