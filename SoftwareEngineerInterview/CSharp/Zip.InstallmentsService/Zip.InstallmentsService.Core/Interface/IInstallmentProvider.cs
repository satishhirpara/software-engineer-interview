using System.Collections.Generic;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Entity.V1.Request;

namespace Zip.InstallmentsService.Core.Interface
{
    /// <summary>
    /// Interface for InstallmentProvider
    /// </summary>
    public interface IInstallmentProvider
    {
        List<InstallmentDto> CalculateInstallments(CreatePaymentPlanRequest requestModel);
    }
}
