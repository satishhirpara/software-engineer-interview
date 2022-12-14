using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Entity.V1.Request;
using Zip.InstallmentsService.Entity.V1.Response;

namespace Zip.InstallmentsService.Core.Interface
{
    /// <summary>
    /// Interface for InstallmentProvider
    /// </summary>
    public interface IInstallmentProvider
    {
        List<InstallmentResponse> CalculateInstallments(CreatePaymentPlanRequest requestModel);
    }
}
