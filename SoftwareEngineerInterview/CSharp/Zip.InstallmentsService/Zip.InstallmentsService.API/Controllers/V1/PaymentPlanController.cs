using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zip.InstallmentsService.API.Helper;
using Zip.InstallmentsService.Core.Interface;
using Zip.InstallmentsService.Entity;
using Zip.InstallmentsService.Entity.V1.Request;

namespace Zip.InstallmentsService.API.Controllers.V1
{

    /// <summary>
    /// Controller which defines all the end points for a purchase installment plan.
    /// </summary>
    [Route("api/[controller]")]
    //[Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class PaymentPlanController : ControllerBase
    {

        private readonly IPaymentPlanProvider _paymentPlanProvider;

        /// <summary>
        /// Intialization in Constructor
        /// </summary>
        /// <param name="paymentPlanProvider"></param>
        public PaymentPlanController(IPaymentPlanProvider paymentPlanProvider)
        {
            _paymentPlanProvider = paymentPlanProvider;
        }

        /// <summary>
        /// Api to get payment plan along with installments by id
        ///  UnComment or add Authorize attribute for JWT token based authentication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        //[Authorize]
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
        public async Task<IActionResult> Post([FromBody] CreatePaymentPlanRequest _requestModel)
        {
            //Create payment plan
            var result = await _paymentPlanProvider.CreatePaymentPlanAsync(_requestModel);
            if (result == null)
            {
                throw new KeyNotFoundException(AppConstants.NoRecordFound);
            }

            return Ok(result);
        }



    }



}
