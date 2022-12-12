using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Core.Interface;
using Zip.InstallmentsService.API.Helper;
using System.Collections.Generic;
using Zip.InstallmentsService.Entity;

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
        /// Api to get payment plan along with installment by id
        ///  UnComment or add Authorize for JWT token based authentication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize]
        [Route("api/PaymentPlan/{id}")]
        public ActionResult<PaymentPlanDto> Get(Guid id)
        {
            //_logger.LogInformation("Start: Get PaymentPlan for id :" + id.ToString());

            //Validate request
            if (id == null || id == Guid.Empty)
            {
                throw new AppException(Constants.BadRequest);
            }

            //Get paymentplan by id
            var result = _paymentPlanProvider.GetById(id);
            if (result == null)
            {
                //_logger.LogInformation("End: Get PaymentPlan for id :" + id.ToString() + "Not found");
                throw new KeyNotFoundException(Constants.NoRecordFound);
            }

            //_logger.LogInformation("End: Get PaymentPlan for id :" + id.ToString());
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
        public ActionResult<PaymentPlanDto> Create(CreatePaymentPlanDto _requestModel)
        {
            _requestModel.Id = Guid.NewGuid();

            if (_requestModel.PurchaseDate == DateTime.MinValue)
                _requestModel.PurchaseDate = DateTime.UtcNow;

            //Validate Request
            var validRequestViewModel = _paymentPlanProvider.ValidateCreateRequest(_requestModel);
            if (!validRequestViewModel.IsValid)
            {
                throw new AppException(validRequestViewModel.Message);
            }

            //Create Plan
            var result = _paymentPlanProvider.Create(_requestModel);
            if (result == null)
            {
                throw new KeyNotFoundException(Constants.NoRecordFound);
            }

            return Ok(result);
        }



    }



}
