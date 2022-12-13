using AutoMapper;
using System;
using System.Linq;
using Zip.InstallmentsService.Data.Interface;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Core.Interface;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Entity.V1.Request;
using Zip.InstallmentsService.Entity.V1.Response;
using Zip.InstallmentsService.Entity.Common;

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
            var response = await _paymentPlanRepository.GetByIdAsync(id);
            if (response == null)
            {
                //_logger.LogInformation();
                return null;
            } 

            return _mapper.Map<PaymentPlanResponse>(response);
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
            if (!validateRequest.IsValid) return null;

            //Calculate installments
            requestModel.Installments = _installmentProvider.CalculateInstallments(requestModel)?.ToList();

            //Create Payment plan
            var paymentPlan =_mapper.Map<PaymentPlan>(requestModel);
            var response = await _paymentPlanRepository.CreatePaymentPlanAsync(paymentPlan);

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
            else if (requestModel.PurchaseAmount <= 0) responemodel.Message = "Please provide valid order amount.";
            else if (requestModel.NoOfInstallments == 0) responemodel.Message = "Please provide valid no of installments.";
            else if (requestModel.FrequencyInDays == 0) responemodel.Message = "Please provide valid frequency.";

            if (!string.IsNullOrEmpty(responemodel.Message)) return responemodel;

            responemodel.IsValid = true;
            return responemodel;
        }

    }
}
