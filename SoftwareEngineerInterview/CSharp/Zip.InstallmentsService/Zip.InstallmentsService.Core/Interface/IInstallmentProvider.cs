using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Entity.V1.Request;

namespace Zip.InstallmentsService.Core.Interface
{
    public interface IInstallmentProvider
    {
        List<InstallmentDto> CalculateInstallments(CreatePaymentPlanRequest requestModel);
    }
}
