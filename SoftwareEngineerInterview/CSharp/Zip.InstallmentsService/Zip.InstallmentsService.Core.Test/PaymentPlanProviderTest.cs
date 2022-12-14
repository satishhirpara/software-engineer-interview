using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// <summary>
    /// Class which defines all the XUnit Test Cases of the PaymentPlanProvider or PaymentPlanService
    /// </summary>
    public class PaymentPlanProviderTest
    {
        private readonly PaymentPlanProvider _sut;
        private readonly Mock<IPaymentPlanRepository> _paymentPlanRepositoryMock = new Mock<IPaymentPlanRepository>();
        private readonly Mock<IInstallmentProvider> _installmentProviderMock = new Mock<IInstallmentProvider>();
        private readonly Mock<ILogger> _logger = new Mock<ILogger>();

        /// <summary>
        /// Intialization in Constructor 
        /// </summary>
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

        /// <summary>
        /// Test case for CreatePaymentPlanAsync Method when valid order amount is provided
        /// </summary>
        [Fact]
        public async void CreatePaymentPlanAsync_ShouldReturnNewCreatedPaymentPlan_WhenValidOrderAmountGiven()
        {
            //Arrange
            var paymentPlanId = Guid.NewGuid();
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

            //--- Mock set up for _installmentProviderMock
            string date = "2022-01-01";
            var createPaymentPlanRequest = this.MockCreatePaymentPlanRequestObject(paymentPlanId, userId, date, 100, 4, 14);
            var installmentResponseList = MockInstallmentResponseList(userId, date);
            _installmentProviderMock.Setup(x => x.CalculateInstallments(createPaymentPlanRequest))
                .Returns(installmentResponseList);

            //--- Mock set up for _paymentPlanRepositoryMock
            var mockPaymentPlan = this.MockPaymentPlanObject(paymentPlanId, userId, date, 100);
            var mockPaymentPlanDto = this.MockPaymentPlanDtoObject(paymentPlanId, userId, date, 100);
            _paymentPlanRepositoryMock.Setup(x => x.CreatePaymentPlanAsync(mockPaymentPlan))
                .ReturnsAsync(mockPaymentPlanDto);

            //Act
            var paymentPlan = await _sut.CreatePaymentPlanAsync(createPaymentPlanRequest);

            //Assert
            Assert.Equal(4, paymentPlan?.Installments?.Count);
        }

        /// <summary>
        /// Test case for CreatePaymentPlanAsync Method when in-valid order amount is provided
        /// </summary>
        [Fact]
        public async void CreatePaymentPlanAsync_ShouldReturnNothing_WhenInValidOrderAmountGiven()
        {
            //Arrange
            var paymentPlanId = Guid.NewGuid();
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            string date = "2022-01-01";
            var paymentPlanMock = this.MockPaymentPlanObject(paymentPlanId, userId, date, 0);
            var createPaymentPlanRequest = this.MockCreatePaymentPlanRequestObject(paymentPlanId, userId, date, 0, 4, 14);
            _paymentPlanRepositoryMock.Setup(x => x.CreatePaymentPlanAsync(paymentPlanMock))
                .ReturnsAsync(() => null);

            //Act
            var paymentPlan = await _sut.CreatePaymentPlanAsync(createPaymentPlanRequest);

            //Assert
            Assert.Null(paymentPlan);
        }

        /// <summary>
        /// Test case for CreatePaymentPlanAsync Method when in-valid input data is provided
        /// </summary>
        [Fact]
        public async void CreatePaymentPlanAsync_ShouldReturnNothing_WhenInValidInputGiven()
        {
            //Arrange
            var paymentPlanId = Guid.NewGuid();
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            string date = "2022-01-01";
            var paymentPlanMock = this.MockPaymentPlanObject(paymentPlanId, userId, date, 0);
            var createPaymentPlanRequest = this.MockCreatePaymentPlanRequestObject(paymentPlanId, userId, date, 0, 0, 0);
            _paymentPlanRepositoryMock.Setup(x => x.CreatePaymentPlanAsync(paymentPlanMock))
                .ReturnsAsync(() => null);

            //Act
            var paymentPlan = await _sut.CreatePaymentPlanAsync(createPaymentPlanRequest);

            //Assert
            Assert.Null(paymentPlan);
        }


        /// <summary>
        /// Test case for GetByIdAsync Method when Payment Plan exist
        /// </summary>
        [Fact]
        public async void GetByIdAsync_ShouldReturnPaymentPlan_WhenPaymentPlanExists()
        {
            //Arrange
            var paymentPlanId = Guid.NewGuid();
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            string date = "2022-01-01";
            var paymentPlanDto = this.MockPaymentPlanDtoObject(paymentPlanId, userId, date, 100);
            _paymentPlanRepositoryMock.Setup(x => x.GetByIdAsync(paymentPlanId))
                .ReturnsAsync(paymentPlanDto);

            //Act
            var paymentPlan = await _sut.GetByIdAsync(paymentPlanId);

            //Assert
            Assert.Equal(paymentPlanId, paymentPlan.Id);
        }

        /// <summary>
        /// Test case for GetByIdAsync Method when Payment Plan does not exist
        /// </summary>
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


        #region Private Methods for mock object preparation

        private PaymentPlanDto MockPaymentPlanDtoObject(Guid id, Guid userId, string date, decimal amount)
        {
            PaymentPlanDto paymentPlanDto = new PaymentPlanDto()
            {
                Id = id,
                UserId = userId,
                PurchaseDate = Convert.ToDateTime(date),
                PurchaseAmount = amount,
                Installments = this.MockInstallmentDtoList(userId, date)
            };
            return paymentPlanDto;
        }
        private PaymentPlan MockPaymentPlanObject(Guid id, Guid userId, string date, decimal amount)
        {
            PaymentPlan paymentPlan = new PaymentPlan()
            {
                Id = id,
                UserId = userId,
                PurchaseDate = Convert.ToDateTime(date),
                PurchaseAmount = amount,
                Installments = this.MockInstallmentList(id, userId, date)
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

        private List<InstallmentDto> MockInstallmentDtoList(Guid userId, string date)
        {
            List<InstallmentDto> installmentDtos = new List<InstallmentDto>()
            {
                new InstallmentDto(){ Id = Guid.NewGuid(), DueDate = Convert.ToDateTime("2022-01-01"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) },
                new InstallmentDto(){ Id = Guid.NewGuid(), DueDate = Convert.ToDateTime("2022-01-15"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) },
                new InstallmentDto(){ Id = Guid.NewGuid(), DueDate = Convert.ToDateTime("2022-01-29"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) },
                new InstallmentDto(){ Id = Guid.NewGuid(), DueDate = Convert.ToDateTime("2022-02-12"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) }
            };

            return installmentDtos;
        }
        private List<InstallmentResponse> MockInstallmentResponseList(Guid userId, string date)
        {
            List<InstallmentResponse> installments = new List<InstallmentResponse>()
             {
                new InstallmentResponse(){ Id = Guid.NewGuid(), DueDate = Convert.ToDateTime("2022-01-01"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) },
                new InstallmentResponse(){ Id = Guid.NewGuid(), DueDate = Convert.ToDateTime("2022-01-15"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) },
                new InstallmentResponse(){ Id = Guid.NewGuid(), DueDate = Convert.ToDateTime("2022-01-29"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) },
                new InstallmentResponse(){ Id = Guid.NewGuid(), DueDate = Convert.ToDateTime("2022-02-12"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) }
            };

            return installments;
        }
        private List<Installment> MockInstallmentList(Guid paymentPlanId, Guid userId, string date)
        {
            List<Installment> installments = new List<Installment>()
            {
                new Installment(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-01"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) },
                new Installment(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-15"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) },
                new Installment(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-29"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) },
                new Installment(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-02-12"), Amount = 25, CreatedBy = userId, CreatedOn = Convert.ToDateTime(date) }
            };

            return installments;
        }

        #endregion


    }
}
