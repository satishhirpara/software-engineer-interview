using System;
using System.Threading.Tasks;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Entity.V1.Request;

namespace Zip.InstallmentsService.Data.Interface
{
    /// <summary>
    /// interface for PaymentPlanRepository
    /// </summary>
    public interface IPaymentPlanRepository
    {
        Task<PaymentPlanDto> CreatePaymentPlanAsync(CreatePaymentPlanRequest requestModel);
        
        Task<PaymentPlanDto> GetByIdAsync(Guid id);
    }
}
