using System;
using System.Threading.Tasks;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Entity.Dto;

namespace Zip.InstallmentsService.Data.Interface
{
    /// <summary>
    /// interface for PaymentPlanRepository
    /// </summary>
    public interface IPaymentPlanRepository
    {
        Task<PaymentPlanDto> CreatePaymentPlanAsync(PaymentPlan requestModel);
        
        Task<PaymentPlanDto> GetByIdAsync(Guid id);
    }
}
