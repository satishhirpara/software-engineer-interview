using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Zip.InstallmentsService.Core.Implementation;
using Zip.InstallmentsService.Core.Interface;
using Zip.InstallmentsService.Core.Mapping;
using Zip.InstallmentsService.Data.Interface;
using Zip.InstallmentsService.Entity.Dto;

namespace Zip.InstallmentsService.Core.Test
{
    public class PaymentPlanProviderTest
    {
        private readonly PaymentPlanProvider _sut;
        private readonly Mock<IPaymentPlanRepository> _paymentPlanRepositoryMock = new Mock<IPaymentPlanRepository>();
        private readonly Mock<IInstallmentProvider> _installmentProviderMock = new Mock<IInstallmentProvider>();
        private readonly Mock<ILogger> _logger = new Mock<ILogger>();
        //private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

        public PaymentPlanProviderTest()
        {
            //auto mapper configuration
            var _mapperMockConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new PaymentPlanProfile());
            });
            var _mapperMock = _mapperMockConfiguration.CreateMapper();

            _sut = new PaymentPlanProvider(_paymentPlanRepositoryMock.Object, _installmentProviderMock.Object, _logger.Object, _mapperMock);
        }

        [Fact]
        public async void GetByIdAsync_ShouldReturnPaymentPlan_WhenPaymentPlanExists()
        {
            //Arrange
            var paymentPlanId = Guid.NewGuid();
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var paymentPlanDto = this.MockPaymentPlanDtoObject(paymentPlanId, userId, "2022-01-01", 100, 4, 14);
            _paymentPlanRepositoryMock.Setup(x => x.GetByIdAsync(paymentPlanId))
                .ReturnsAsync(paymentPlanDto);

            //Act
            var paymentPlan = await _sut.GetByIdAsync(paymentPlanId);

            //Assert
            Assert.Equal(paymentPlanId, paymentPlan.Id);
        }

        [Fact]
        public async void GetByIdAsync_ShouldReturnNothing_WhenPaymentPlanDoesNotExist()
        {
            //Arrange
            _paymentPlanRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            //Act
            var paymentPlan = await _sut.GetByIdAsync(Guid.NewGuid());

            //Assert
            Assert.Null(paymentPlan);
        }


        #region Private Methods

        private PaymentPlanDto MockPaymentPlanDtoObject(Guid id, Guid userId, string date, decimal amount, int noOfInstallments, int frequencyInDays)
        {
            PaymentPlanDto paymentPlanDto = new PaymentPlanDto()
            {
                Id = id,
                UserId = userId,
                PurchaseDate = Convert.ToDateTime(date),
                PurchaseAmount = amount,
                NoOfInstallments = noOfInstallments,
                FrequencyInDays = frequencyInDays,
                Installments = this.MockInstallments(id)
            };

            return paymentPlanDto;
        }

        private List<InstallmentDto> MockInstallments(Guid paymentPlanId)
        {
            List<InstallmentDto> installmentDtos = new List<InstallmentDto>()
            {
                new InstallmentDto(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-01"), Amount = 25 },
                new InstallmentDto(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-15"), Amount = 25 },
                new InstallmentDto(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-29"), Amount = 25 },
                new InstallmentDto(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-02-12"), Amount = 25 }
            };

            return installmentDtos;
        }

        #endregion


    }
}
