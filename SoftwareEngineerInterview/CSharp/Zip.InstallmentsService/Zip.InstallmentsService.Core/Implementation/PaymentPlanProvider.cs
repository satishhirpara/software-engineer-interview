using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zip.InstallmentsService.Core.Extension;
using Zip.InstallmentsService.Core.Interface;
using Zip.InstallmentsService.Data.Interface;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Entity.Common;
using Zip.InstallmentsService.Entity.V1.Request;
using Zip.InstallmentsService.Entity.V1.Response;

namespace Zip.InstallmentsService.Core.Implementation
{
    /// <summary>
    /// Core class which defines all bussiness logic for a payment plan.
    /// </summary>
    public class PaymentPlanProvider : IPaymentPlanProvider
    {
        private readonly IPaymentPlanRepository _paymentPlanRepository;
        private readonly IInstallmentProvider _installmentProvider;
        private readonly ILogger _logger;
        private IMapper _mapper { get; }

        /// <summary>
        /// Intialization in Constructor
        /// </summary>
        /// <param name="paymentPlanRepository"></param>
        /// <param name="installmentProvider"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public PaymentPlanProvider(IPaymentPlanRepository paymentPlanRepository, IInstallmentProvider installmentProvider, ILogger logger, IMapper mapper)
        {
            _paymentPlanRepository = paymentPlanRepository;
            _installmentProvider = installmentProvider;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get Payment plan by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PaymentPlanResponse> GetByIdAsync(Guid id)
        {
            _logger.LogDebug("Request received to get a payment plan for Id: {Id}", id);

            var paymentPlan = await _paymentPlanRepository.GetByIdAsync(id);
            if (paymentPlan == null)
            {
                _logger.LogInformation("Unable to find a payment plan with Id: {Id}", id);
                return null;
            }

            _logger.LogDebug("Retrieved a payment plan with Id: {Id}", id);
            return _mapper.Map<PaymentPlanResponse>(paymentPlan);
        }

        /// <summary>
        /// Logic to create payment plan
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public async Task<PaymentPlanResponse> CreatePaymentPlanAsync(CreatePaymentPlanRequest requestModel)
        {
            _logger.LogDebug("Request received to create a payment plan with Id:{Id}, userId:{userId}, orderAmount:{orderAmount},orderDate:{orderDate},NoOfInstallments:{NoOfInstallments},FrequencyInDays:{FrequencyInDays}",
                requestModel.Id, requestModel.UserId, requestModel.PurchaseAmount, requestModel.PurchaseDate, requestModel.NoOfInstallments, requestModel.FrequencyInDays);

            //Set values if needed
            if (requestModel.Id == Guid.Empty) requestModel.Id = Guid.NewGuid();
            if (requestModel.PurchaseDate == DateTime.MinValue) requestModel.PurchaseDate = DateTime.UtcNow.Date;

            //Logic to Calculate installments
            requestModel.Installments = _installmentProvider.CalculateInstallments(requestModel)?.ToList();
         
            //Create Payment plan
            var response = await _paymentPlanRepository.CreatePaymentPlanAsync(requestModel);
            if (response == null)
            {
                _logger.LogError("An error has occurred while creating a payment plan for : {userId} with Id: {Id}", requestModel.UserId, requestModel.Id);
                return null;
            }

            _logger.LogDebug("Payment plan created successfully for Id:{Id}, userId:{userId}, orderAmount:{orderAmount},orderDate:{orderDate},NoOfInstallments:{NoOfInstallments},FrequencyInDays:{FrequencyInDays}",
                requestModel.Id, requestModel.UserId, requestModel.PurchaseAmount, requestModel.PurchaseDate, requestModel.NoOfInstallments, requestModel.FrequencyInDays);
            return _mapper.Map<PaymentPlanResponse>(response);
        }

        private List<InstallmentResponse> CalculateInstallments(CreatePaymentPlanRequest requestModel)
        {
            List<InstallmentResponse> installments = new List<InstallmentResponse>();

            // Logic to calculate installment amount as per no of installments
            var purchaseDate = requestModel.PurchaseDate;
            var purchaseAmount = requestModel.PurchaseAmount;
            var noOfInstallments = requestModel.NoOfInstallments;
            var frequencyInDays = requestModel.FrequencyInDays;
            var installmentAmount = this.GetNextInstallmentAmount(purchaseAmount, noOfInstallments);

            //Loop through noOfInstallments and prepare installments
            InstallmentResponse installment;
            var nextInstallmentDate = purchaseDate;
            for (int i = 1; i <= requestModel.NoOfInstallments; i++)
            {
                installment = new InstallmentResponse();
                installment.Id = System.Guid.NewGuid();

                //Logic to get next installment date after frequency days
                if (i > 1) nextInstallmentDate = nextInstallmentDate.GetNextDateAfterDays(frequencyInDays);
                installment.DueDate = nextInstallmentDate.Date;
                installment.Amount = installmentAmount;

                installment.CreatedOn = DateTime.UtcNow;
                installment.CreatedBy = requestModel.UserId;

                installments.Add(installment);
            }

            return installments;
        }
        private decimal GetNextInstallmentAmount(decimal purchaseAmount, int noOfInstallments)
        {
            decimal result = 0;
            if (noOfInstallments == 0) return result;
            decimal installmentAmount = Convert.ToDecimal(purchaseAmount / noOfInstallments);
            return installmentAmount;
        }

    }
}
