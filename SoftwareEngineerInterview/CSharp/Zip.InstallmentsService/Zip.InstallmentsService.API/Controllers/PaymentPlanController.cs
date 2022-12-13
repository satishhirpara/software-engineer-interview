using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zip.InstallmentsService.API.Helper;
using Zip.InstallmentsService.Core.Interface;
using Zip.InstallmentsService.Entity;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Entity.V1.Request;

namespace Zip.InstallmentsService.API.Controllers
{

    /// <summary>
    /// Controller which defines all the end points for a purchase installment plan.
    /// </summary>
    [ApiController]
    public class PaymentPlanController : ControllerBase
    {

        private readonly IPaymentPlanProvider _paymentPlanProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// Intialization in Constructor
        /// </summary>
        /// <param name="paymentPlanProvider"></param>
        /// <param name="_logger"></param>
        public PaymentPlanController(IPaymentPlanProvider paymentPlanProvider, ILogger logger)
        {
            _paymentPlanProvider = paymentPlanProvider;
            _logger = logger;
        }

        /// <summary>
        /// Api to get payment plan along with installments by id
        ///  UnComment or add Authorize attribute for JWT token based authentication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize]
        [Route("api/PaymentPlan/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            //Validate request
            if (id == null || id == Guid.Empty)
            {
                throw new AppException(AppConstants.BadRequest);
            }

            //Get paymentplan by id
            var result = await _paymentPlanProvider.GetByIdAsync(id);
            if (result == null)
            {
                throw new KeyNotFoundException(AppConstants.NoRecordFound);
            }
            
            return Ok(result);
        }


        /// <summary>
        /// Api to create payment plan intallments,
        /// UnComment or add Authorize attribute for JWT token based authentication
        /// </summary>
        /// <param name="_requestModel"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize] 
        [Route("api/PaymentPlan")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentPlanRequest _requestModel)
        {
            if (_requestModel.Id == Guid.Empty) _requestModel.Id = Guid.NewGuid();

            if (_requestModel.PurchaseDate == DateTime.MinValue)
                _requestModel.PurchaseDate = DateTime.UtcNow;

            //Validate Request
            var validRequestViewModel = _paymentPlanProvider.ValidateCreatePaymentPlanRequest(_requestModel);
            if (!validRequestViewModel.IsValid)
            {
                throw new AppException(validRequestViewModel.Message);
            }

            //Create Plan
            var result = _paymentPlanProvider.CreatePaymentPlanAsync(_requestModel);
            if (result == null)
            {
                throw new KeyNotFoundException(AppConstants.NoRecordFound);
            }

            return Ok(result);
        }



    }



}
