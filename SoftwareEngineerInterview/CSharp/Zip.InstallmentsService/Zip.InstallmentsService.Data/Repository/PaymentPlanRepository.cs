using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zip.InstallmentsService.Data.Interface;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Entity.V1.Request;

namespace Zip.InstallmentsService.Data.Repository
{
    /// <summary>
    /// This Repository class contains all methods or dataAcess logic for payment plan
    /// </summary>
    public class PaymentPlanRepository : IPaymentPlanRepository
    {
        private readonly ApiContext _context;

        /// <summary>
        /// Intialization in Constructor 
        /// </summary>
        /// <param name="context"></param>
        public PaymentPlanRepository(ApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Database logic to get payment plan with all installments by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PaymentPlanDto> GetByIdAsync(Guid id)
        {
            var paymentPlan = await _context.PaymentPlans.Where(k => k.Id == id).Include(k => k.Installments)?.FirstOrDefaultAsync();
            if (paymentPlan == null) return null;

            return this.MapToPaymentPlanDto(paymentPlan);
        }

        /// <summary>
        /// Database logic to create payment plan with all installments
        /// </summary>
        /// <param name="_paymentPlan"></param>
        /// <returns></returns>
        public async Task<PaymentPlanDto> CreatePaymentPlanAsync(CreatePaymentPlanRequest requestModel)
        {
            PaymentPlan paymentPlan = new PaymentPlan()
            {
                Id = requestModel.Id,
                UserId = requestModel.UserId,
                PurchaseAmount = requestModel.PurchaseAmount,
                PurchaseDate = requestModel.PurchaseDate,
                CreatedOn = DateTime.UtcNow.Date,
                CreatedBy = requestModel.UserId
            };
            
            List<Installment> finalList = new List<Installment>();
            foreach (var item in requestModel.Installments)
            {
                var installment = new Installment()
                {
                    Id = item.Id,
                    DueDate = item.DueDate,
                    Amount = item.Amount,
                    CreatedOn = item.CreatedOn,
                    CreatedBy = item.CreatedBy,
                    PaymentPlanId = paymentPlan.Id
                };

                finalList.Add(installment);
            }
            paymentPlan.Installments = finalList;

            var result = await _context.PaymentPlans.AddAsync(paymentPlan);
            await _context.SaveChangesAsync();
            return this.MapToPaymentPlanDto(result.Entity); 
        }

        /// <summary>
        /// Map PaymentPlan object to PaymentPlanDto object
        /// </summary>
        /// <param name="paymentPlan"></param>
        /// <returns></returns>
        private PaymentPlanDto MapToPaymentPlanDto(PaymentPlan paymentPlan)
        {
            //get installments
            var installments = paymentPlan?.Installments.Select(item => new InstallmentDto
            {
                Id = item.Id,
                PaymentPlanId = item.PaymentPlanId,
                DueDate = item.DueDate,
                Amount = item.Amount,
                CreatedOn = item.CreatedOn,
                CreatedBy = item.CreatedBy
            });

            return new PaymentPlanDto()
            {
                Id = paymentPlan.Id,
                UserId = paymentPlan.UserId,
                PurchaseAmount = paymentPlan.PurchaseAmount,
                PurchaseDate = paymentPlan.PurchaseDate,
                Installments = installments.ToList() ?? null
            };
        }


    }
}
