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
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Entity.Dto;
using Zip.InstallmentsService.Entity.V1.Request;
using Zip.InstallmentsService.Entity.V1.Response;

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
        public async void CreatePaymentPlanAsync_ShouldReturnNewCreatedPaymentPlan_WhenValidOrderAmountGiven()
        {
            //Arrange
            var paymentPlanId = Guid.NewGuid();
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

           //--- Mock set up for _installmentProviderMock
            var createPaymentPlanRequest = this.MockCreatePaymentPlanRequestObject(paymentPlanId, userId, "2022-01-01", 100, 4, 14);
            var installmentResponseList = MockInstallmentResponseList(paymentPlanId);
            _installmentProviderMock.Setup(x => x.CalculateInstallments(createPaymentPlanRequest))
                .Returns(installmentResponseList);

            //--- Mock set up for _paymentPlanRepositoryMock
            var mockPaymentPlan = this.MockPaymentPlanObject(paymentPlanId, userId, "2022-01-01", 100, 4, 14);
            var mockPaymentPlanDto = this.MockPaymentPlanDtoObject(paymentPlanId, userId, "2022-01-01", 100, 4, 14);
            _paymentPlanRepositoryMock.Setup(x => x.CreatePaymentPlanAsync(mockPaymentPlan))
                .ReturnsAsync(mockPaymentPlanDto);

            //Act
            var paymentPlan = await _sut.CreatePaymentPlanAsync(createPaymentPlanRequest);

            //Assert
            Assert.Equal(4, paymentPlan.Installments.Count);
        }

        [Fact]
        public async void CreatePaymentPlanAsync_ShouldReturnNothing_WhenInValidOrderAmountGiven()
        {
            //Arrange
            var paymentPlanId = Guid.NewGuid();
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var paymentPlanMock = this.MockPaymentPlanObject(paymentPlanId, userId, "2022-01-01", 0, 4, 14);
            var createPaymentPlanRequest = this.MockCreatePaymentPlanRequestObject(paymentPlanId, userId, "2022-01-01", 0, 4, 14);
            _paymentPlanRepositoryMock.Setup(x => x.CreatePaymentPlanAsync(paymentPlanMock))
                .ReturnsAsync(() => null);

            //Act
            var paymentPlan = await _sut.CreatePaymentPlanAsync(createPaymentPlanRequest);

            //Assert
            Assert.Null(paymentPlan);
        }

        [Fact]
        public async void CreatePaymentPlanAsync_ShouldReturnNothing_WhenInValidInputGiven()
        {
            //Arrange
            var paymentPlanId = Guid.NewGuid();
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var paymentPlanMock = this.MockPaymentPlanObject(paymentPlanId, userId, "2022-01-01", 0, 4, 14);
            var createPaymentPlanRequest = this.MockCreatePaymentPlanRequestObject(paymentPlanId, userId, "2022-01-01", 0, 0, 0);
            _paymentPlanRepositoryMock.Setup(x => x.CreatePaymentPlanAsync(paymentPlanMock))
                .ReturnsAsync(() => null);

            //Act
            var paymentPlan = await _sut.CreatePaymentPlanAsync(createPaymentPlanRequest);

            //Assert
            Assert.Null(paymentPlan);
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
            Assert.Null(paymentPlan);
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
                Installments = this.MockInstallmentDtoList(id)
            };
            return paymentPlanDto;
        }
        private PaymentPlan MockPaymentPlanObject(Guid id, Guid userId, string date, decimal amount, int noOfInstallments, int frequencyInDays)
        {
            PaymentPlan paymentPlan = new PaymentPlan()
            {
                Id = id,
                UserId = userId,
                PurchaseDate = Convert.ToDateTime(date),
                PurchaseAmount = amount,
                NoOfInstallments = noOfInstallments,
                FrequencyInDays = frequencyInDays,
                Installments = this.MockInstallmentList(id)
            };
            return paymentPlan;
        }

        private CreatePaymentPlanRequest MockCreatePaymentPlanRequestObject(Guid id, Guid userId, string date, decimal amount, int noOfInstallments, int frequencyInDays)
        {
            CreatePaymentPlanRequest createPaymentPlanRequest = new CreatePaymentPlanRequest()
            {
                Id = id,
                UserId = userId,
                PurchaseDate = Convert.ToDateTime(date),
                PurchaseAmount = amount,
                NoOfInstallments = noOfInstallments,
                FrequencyInDays = frequencyInDays
            };

            return createPaymentPlanRequest;
        }

        private List<InstallmentDto> MockInstallmentDtoList(Guid paymentPlanId)
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
        private List<InstallmentResponse> MockInstallmentResponseList(Guid paymentPlanId)
        {
            List<InstallmentResponse> installments = new List<InstallmentResponse>()
            {
                new InstallmentResponse(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-01"), Amount = 25 },
                new InstallmentResponse(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-15"), Amount = 25 },
                new InstallmentResponse(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-29"), Amount = 25 },
                new InstallmentResponse(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-02-12"), Amount = 25 }
            };

            return installments;
        }
        private List<Installment> MockInstallmentList(Guid paymentPlanId)
        {
            List<Installment> installments = new List<Installment>()
            {
                new Installment(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-01"), Amount = 25 },
                new Installment(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-15"), Amount = 25 },
                new Installment(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-29"), Amount = 25 },
                new Installment(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-02-12"), Amount = 25 }
            };

            return installments;
        }

        #endregion


    }
}
