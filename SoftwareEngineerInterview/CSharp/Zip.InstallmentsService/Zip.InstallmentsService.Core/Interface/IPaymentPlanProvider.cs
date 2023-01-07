using System;
using System.Threading.Tasks;
using Zip.InstallmentsService.Entity.V1.Request;
using Zip.InstallmentsService.Entity.V1.Response;

namespace Zip.InstallmentsService.Core.Interface
{
    /// <summary>
    /// interface for PaymentPlanProvider
    /// </summary>
    public interface IPaymentPlanProvider
    {
        Task<PaymentPlanResponse> CreatePaymentPlanAsync(CreatePaymentPlanRequest requestModel);
        Task<PaymentPlanResponse> GetByIdAsync(Guid id);

    }
}
