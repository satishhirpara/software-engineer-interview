using System;
using System.Collections.Generic;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Core.Helper;
using Zip.InstallmentsService.Core.Interface;
using System.Threading.Tasks;
using Zip.InstallmentsService.Entity.V1.Request;
using Zip.InstallmentsService.Entity.V1.Response;

namespace Zip.InstallmentsService.Core.Implementation
{
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
        public List<InstallmentResponse> CalculateInstallments(CreatePaymentPlanRequest requestModel)
        {
            List<InstallmentResponse> installments = new List<InstallmentResponse>();

            var purchaseDate = requestModel.PurchaseDate; // 01-01-2022
            var purchaseAmount = requestModel.PurchaseAmount; // $100.00
            var noOfInstallments = requestModel.NoOfInstallments; // 4
            var frequencyInDays = requestModel.FrequencyInDays; // 14 days
            var installmentAmount = this.GetNextInstallmentAmount(purchaseAmount, noOfInstallments, frequencyInDays);

            InstallmentResponse installment;
            var nextInstallmentDate = purchaseDate;
            for (int i = 1; i <= requestModel.NoOfInstallments; i++)
            {
                installment = new InstallmentResponse();
                installment.Id = System.Guid.NewGuid();
                installment.PaymentPlanId = requestModel.Id;

                if (i > 1) nextInstallmentDate = DateTimeHelper.GetNextDateAfterDays(nextInstallmentDate, frequencyInDays);
                installment.DueDate = nextInstallmentDate.Date;
                installment.Amount = installmentAmount;

                installment.CreatedOn = DateTime.UtcNow;
                installment.CreatedBy = requestModel.UserId;

                installments.Add(installment);
            }

            return installments;
        }

        /// <summary>
        /// Logic to get next installment amount
        /// </summary>
        /// <param name="purchaseAmount"></param>
        /// <param name="noOfInstallments"></param>
        /// <param name="frequencyInDays"></param>
        /// <returns></returns>
        private decimal GetNextInstallmentAmount(decimal purchaseAmount, int noOfInstallments, int frequencyInDays)
        {
            decimal result = 0;
            if (noOfInstallments == 0) return result;
            decimal installmentAmount = Convert.ToDecimal(purchaseAmount / noOfInstallments);
            return installmentAmount;
        }

    }
}
