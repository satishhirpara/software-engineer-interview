using System;
using System.Collections.Generic;
using Zip.InstallmentsService.Core.Extension;
using Zip.InstallmentsService.Core.Interface;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Entity.V1.Request;

namespace Zip.InstallmentsService.Core.Implementation
{
    /// <summary>
    /// This class contains all methods or bussiness logic for installation
    /// </summary>
    public class InstallmentProvider : IInstallmentProvider
    {
        /// <summary>
        /// Intialization in Constructor
        /// </summary>
        public InstallmentProvider()
        {

        }

        /// <summary>
        /// Logic to calculate installments as per no of installments and frequency
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public List<InstallmentDto> CalculateInstallments(CreatePaymentPlanRequest requestModel)
        {
            List<InstallmentDto> installments = new List<InstallmentDto>();

            // Logic to calculate installment amount as per no of installments
            var installmentAmount = this.GetNextInstallmentAmount(requestModel.PurchaseAmount, requestModel.NoOfInstallments);

            //Loop through noOfInstallments and prepare installments
            InstallmentDto installment;
            var nextInstallmentDate = requestModel.PurchaseDate;
            for (int i = 1; i <= requestModel.NoOfInstallments; i++)
            {
                installment = new InstallmentDto(Guid.NewGuid(), requestModel.Id, null, installmentAmount, DateTime.UtcNow, requestModel.UserId);

                //Logic to get next installment date after frequency days
                if (i > 1) nextInstallmentDate = nextInstallmentDate.GetNextDateAfterDays(requestModel.FrequencyInDays);
                installment.DueDate = nextInstallmentDate.Date;
                installments.Add(installment);
            }

            return installments;
        }

        /// <summary>
        /// Logic to get next installment amount
        /// </summary>
        /// <param name="purchaseAmount"></param>
        /// <param name="noOfInstallments"></param>
        /// <returns></returns>
        private decimal GetNextInstallmentAmount(decimal purchaseAmount, int noOfInstallments)
        {
            if (noOfInstallments == 0) return 0;
            decimal installmentAmount = Convert.ToDecimal(purchaseAmount / noOfInstallments);
            return installmentAmount;
        }

    }
}
