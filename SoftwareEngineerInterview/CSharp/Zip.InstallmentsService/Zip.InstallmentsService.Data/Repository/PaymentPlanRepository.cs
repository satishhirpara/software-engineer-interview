using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zip.InstallmentsService.Data.Interface;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Entity.Dto;

namespace Zip.InstallmentsService.Data.Repository
{
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
            var paymentPlan = _context.PaymentPlans.Where(k => k.Id == id).Include(k => k.Installments)?.FirstOrDefault();
            return this.MapToPaymentPlanDto(paymentPlan);
        }

        /// <summary>
        /// Database logic to create payment plan with all installments
        /// </summary>
        /// <param name="_paymentPlan"></param>
        /// <returns></returns>
        public async Task<PaymentPlanDto> CreatePaymentPlanAsync(PaymentPlan paymentPlan)
        {
            var result = await _context.PaymentPlans.AddAsync(paymentPlan);
            await _context.SaveChangesAsync();
            return this.MapToPaymentPlanDto(result.Entity); 
        }

        private PaymentPlanDto MapToPaymentPlanDto(PaymentPlan paymentPlan)
        {
            //get installments
            var installments = paymentPlan?.Installments.Select(item => new InstallmentDto
            {
                Id = item.Id,
                PaymentPlanId = item.PaymentPlanId,
                DueDate = item.DueDate,
                Amount = item.Amount
            });

            return new PaymentPlanDto()
            {
                Id = paymentPlan.Id,
                UserId = paymentPlan.UserId,
                PurchaseAmount = paymentPlan.PurchaseAmount,
                PurchaseDate = paymentPlan.PurchaseDate,
                NoOfInstallments = paymentPlan.NoOfInstallments,
                FrequencyInDays = paymentPlan.FrequencyInDays,
                Installments = installments.ToList() ?? null
            };
        }


    }
}
