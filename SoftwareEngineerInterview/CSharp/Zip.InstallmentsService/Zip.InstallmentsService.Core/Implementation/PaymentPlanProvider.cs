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
            //Validate request
            var validateRequest = this.ValidateCreatePaymentPlanRequest(requestModel);
            if (!validateRequest.IsValid)
            {
                _logger.LogInformation("Unable to create a payment plan due to bad request for userId : {userId}", requestModel.UserId);
                return null;
            }

            //Calculate installments
            var paymentPlan = _mapper.Map<PaymentPlan>(requestModel);
            var installments = _installmentProvider.CalculateInstallments(requestModel)?.ToList();
            paymentPlan.Installments = _mapper.Map<List<Installment>>(installments);
            
            //Create Payment plan
            var response = await _paymentPlanRepository.CreatePaymentPlanAsync(paymentPlan);

            _logger.LogInformation("Payment plan created successfully for user : {userId} with Id: {Id}", requestModel.UserId, requestModel.Id);
            return _mapper.Map<PaymentPlanResponse>(response);
        }


        /// <summary>
        /// Validate Payment plan create request
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ValidateRequest ValidateCreatePaymentPlanRequest(CreatePaymentPlanRequest requestModel)
        {
            var responemodel = new ValidateRequest();
            if (requestModel == null) responemodel.Message = "Bad Request.";
            else if (requestModel.UserId == Guid.Empty) responemodel.Message = "Please provide userid.";
            else if (requestModel.PurchaseAmount <= 0) responemodel.Message = "Please provide valid order amount.";
            else if (requestModel.NoOfInstallments == 0) responemodel.Message = "Please provide valid no of installments.";
            else if (requestModel.PurchaseAmount <= requestModel.NoOfInstallments) responemodel.Message = "Please provide valid order amount or no of installments.";
            else if (requestModel.FrequencyInDays == 0 || requestModel.FrequencyInDays > 365) responemodel.Message = "Please provide valid frequency between 0 to 365 days.";

            if (!string.IsNullOrEmpty(responemodel.Message)) return responemodel;

            responemodel.IsValid = true;
            return responemodel;
        }

    }
}
