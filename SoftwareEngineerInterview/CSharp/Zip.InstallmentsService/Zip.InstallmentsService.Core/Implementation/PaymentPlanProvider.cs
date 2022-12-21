using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var paymentPlan = await _paymentPlanRepository.GetByIdAsync(id);
            if (paymentPlan == null)
            {
                _logger.LogInformation("Unable to find a payment plan with Id: {Id}", id);
                return null;
            }

            _logger.LogInformation("Retrieved a payment plan with Id: {Id}", id);
            return _mapper.Map<PaymentPlanResponse>(paymentPlan);
        }

        /// <summary>
        /// Logic to create payment plan
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public async Task<PaymentPlanResponse> CreatePaymentPlanAsync(CreatePaymentPlanRequest requestModel)
        {
            _logger.LogInformation("Request received to create a payment plan for user : {userId} with Id: {Id}", requestModel.UserId, requestModel.Id);

            //Logic to Calculate installments
            var paymentPlan = _mapper.Map<PaymentPlan>(requestModel);
            var installments = _installmentProvider.CalculateInstallments(requestModel)?.ToList();
            paymentPlan.Installments = _mapper.Map<List<Installment>>(installments);

            //Create Payment plan
            var response = await _paymentPlanRepository.CreatePaymentPlanAsync(paymentPlan);

            _logger.LogInformation("Payment plan created successfully for user : {userId} with Id: {Id}", requestModel.UserId, requestModel.Id);
            return _mapper.Map<PaymentPlanResponse>(response);
        }

    }
}
