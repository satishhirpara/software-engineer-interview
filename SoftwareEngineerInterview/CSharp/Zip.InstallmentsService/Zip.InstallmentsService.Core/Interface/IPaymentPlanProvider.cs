using System;
using System.Threading.Tasks;
using Zip.InstallmentsService.Entity.Common;
using Zip.InstallmentsService.Entity.V1.Request;
using Zip.InstallmentsService.Entity.V1.Response;

namespace Zip.InstallmentsService.Core.Interface
{
    public interface IPaymentPlanProvider
    {
        ValidateRequest ValidateCreatePaymentPlanRequest(CreatePaymentPlanRequest requestModel);
        Task<PaymentPlanResponse> CreatePaymentPlanAsync(CreatePaymentPlanRequest requestModel);
        Task<PaymentPlanResponse> GetByIdAsync(Guid id);

    }
}
